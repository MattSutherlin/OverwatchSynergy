var HeroViewModel = function (hero, calculatorViewModel, team) {
    this.Name = hero.Name;

    this.AddAsOpponent = function () {
        calculatorViewModel.Opponents.push(new HeroViewModel(hero, calculatorViewModel, calculatorViewModel.Opponents));
    }

    this.AddAsTeammate = function () {
        calculatorViewModel.Teammates.push(new HeroViewModel(hero, calculatorViewModel, calculatorViewModel.Teammates));
    }

    this.Remove = function () {
        if (team && typeof(team.remove) === 'function') {
            team.remove(this);
        }
    }
}

var CalculatorViewModel = function (heroesJson) {
    var _this = this,
        _counterWeights = ko.observable([]),
        _synergyWeights = ko.observable([]);

    this.Opponents = ko.observableArray();
    this.Teammates = ko.observableArray();
    this.AvailableHeroes = heroesJson.map(function (hero) {
        return new HeroViewModel(hero, _this);
    });
    this.WeightedSuggestions = ko.computed(function () {
        var counterWeights = ko.unwrap(_counterWeights),
            synergyWeights = ko.unwrap(_synergyWeights),
            allWeights = [];
        for (hero of heroesJson) {
            allWeights.push({
                Name: hero.Name,
                Weight: combineWeights(counterWeights[hero.Name], synergyWeights[hero.Name])
            });
        }
        return allWeights.sort(function(a,b){return b.Weight - a.Weight;});
    });

    function getWeightMap(weights) {
        var map = {};
        for (weight of weights) {
            map[weight.Hero.Name] = weight.Value;
        }
        return map;
    }

    function combineWeights(counter, synergy) {
        if (counter === undefined && synergy === undefined) {
            return 50;
        }
        if (counter === undefined) {
            return synergy;
        }
        if (synergy === undefined) {
            return counter;
        }
        return (counter + synergy) / 2
    }

    this.Opponents.subscribe(function (opponents) {
        $.post({
            url: "../calculator/GetHeroesStrengthAgainst",
            data: JSON.stringify(opponents.map(function (h) { return h.Name; })),
            contentType: "application/json"
        })
        .done(function (data) {
            _counterWeights(getWeightMap(data));
        })
    });

    this.Teammates.subscribe(function (teammates) {
        $.post({
            url: "../calculator/GetHeroesThatHaveSynergiesWith",
            data: JSON.stringify(teammates.map(function (h) { return h.Name; })),
            contentType: "application/json"
        })
        .done(function (data) {
            _synergyWeights(getWeightMap(data));
        })
    });

    
};

$(document).ready(function () {
    $.getJSON("../heroes/")
        .done(function (data) {
            ko.applyBindings(new CalculatorViewModel(data));
        });
});