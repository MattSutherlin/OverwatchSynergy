using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using OverwatchSynergy.Api;
using OverwatchSynergy.Api.Heroes;

namespace OverwatchSynergy.Tests
{
    [TestFixture]
    public class OverallScoreTests
    {
        [Test]
        public void overallscores_can_be_retrieved()
        {
            var overallscores = Calculator.GetOverallScoresForAllHeroes(new[] {new Reaper()}, new[] {new Torbjorn()}, 1);

            overallscores.Single(s => s.Hero is Winston).Value.Should().Be(75);
        }

        [Test]
        public void a_good_team_is_suggested_against_team_tobjorn()
        {
            var tobjornTeam = new[]{new Torbjorn(),new Torbjorn(),new Torbjorn(),new Torbjorn(),new Torbjorn(),new Torbjorn()};

            Calculator.GetOverallScoresForAllHeroes(tobjornTeam, new Hero[0], 1)
                      .Take(6).Select(w => w.Hero.Id).Should().BeEquivalentTo(new[]
                                                                      {
                                                                          new Genji().Id,
                                                                          new Roadhog().Id,
                                                                          new Lucio().Id,
                                                                          new McCree().Id,
                                                                          new Pharah().Id,
                                                                          new Soldier76().Id
                                                                      });
        }

        [Test]
        public void a_good_team_is_iteratively_suggested_against_team_tobjorn()
        {
            var tobjornTeam = new[] {new Torbjorn(), new Torbjorn(), new Torbjorn(), new Torbjorn(), new Torbjorn(), new Torbjorn()};

            Calculator.GetOverallScoresForAllHeroes(tobjornTeam, new Hero[0], 1)
                      .First().Hero.Id.Should().Be(new Genji().Id);
            Calculator.GetOverallScoresForAllHeroes(tobjornTeam, new Hero[] {new Genji()}, 1)
                      .First().Hero.Id.Should().Be(new Lucio().Id);
            Calculator.GetOverallScoresForAllHeroes(tobjornTeam, new Hero[] {new Genji(), new Lucio()}, 1)
                      .First().Hero.Id.Should().Be(new Roadhog().Id);
            Calculator.GetOverallScoresForAllHeroes(tobjornTeam, new Hero[] {new Genji(), new Lucio(), new Roadhog(),}, 1)
                      .First().Hero.Id.Should().Be(new McCree().Id);
            Calculator.GetOverallScoresForAllHeroes(tobjornTeam, new Hero[] {new Genji(), new Lucio(), new Roadhog(), new McCree()}, 1)
                      .First().Hero.Id.Should().Be(new Pharah().Id);
            Calculator.GetOverallScoresForAllHeroes(tobjornTeam, new Hero[] {new Genji(), new Lucio(), new Roadhog(), new McCree(), new Pharah()}, 1)
                      .First().Hero.Id.Should().Be(new Mercy().Id);
        }
    }
}