using Engine.utils;
using Engine.managers;
using Engine.models;

namespace EngineTester
{
    [TestClass]
    public sealed class ConfigLoaderTests
    {
        private const string TestValidCardsConfigPath = "test_configs/valid_cards.json";
        private const string TestValidFinancesConfigPath = "test_configs/valid_finances.json";
        private const string TestValidGalaxyConfigPath = "test_configs/valid_galaxy.json";
        private const string TestNonExistentConfigPath = "test_configs/non_existent.json";
        private const string TestInValidFinancesConfigPath = "test_configs/invalid_finances.json";
        private const string TestInValidGalaxyConfigPath = "test_configs/invalid_galaxy.json";
        private const string TestInValidCardsConfigPath = "test_configs/invalid_cards.json";

        // 55 Asserts

        [TestMethod]
        public void LoadFinanceManagerConfig_ValidFile_ReturnsCorrectFinanceManager()
        {
            FinanceManager? financeManager = ConfigLoader.LoadFinanceManagerConfig(TestValidFinancesConfigPath);
            Assert.IsNotNull(financeManager);

            // Asercje dla pasywnych dochodów
            Assert.AreEqual(500, financeManager.passiveBase, "PassiveBase should be 500.");
            Assert.AreEqual(1000, financeManager.passiveByGalacticShipyard, "PassiveGalacticShipyard should be 1000.");
            CollectionAssert.AreEqual(new List<int> { 50, 120, 250 }, financeManager.passiveByMineLevel, "PassiveMine should match.");
            CollectionAssert.AreEqual(new List<int> { 50, 110, 220, 350, 450, 550 }, financeManager.passiveByFarmLevel, "PassiveFarm should match.");
            CollectionAssert.AreEqual(new List<int> { 250, 400, 600 }, financeManager.passiveByAsteroidMineLevel, "PassiveAsteroidMine should match.");

            // Asercje dla kosztów budowy/rozbudowy
            CollectionAssert.AreEqual(new List<int> { 300, 600, 1000, 1350, 1600 }, financeManager.costbyHotelLevel, "CostHotel should match.");
            CollectionAssert.AreEqual(new List<int> { 500, 800, 1200, 1500, 2000 }, financeManager.costUpgradeHotel, "CostUpgradeHotel should match.");
            CollectionAssert.AreEqual(new List<int> { 300, 600, 900 }, financeManager.costUpgradeMine, "CostUpgradeMine should match.");
            CollectionAssert.AreEqual(new List<int> { 200, 400, 600, 800, 1000, 1200 }, financeManager.costUpgradeFarm, "CostUpgradeFarm should match.");
            CollectionAssert.AreEqual(new List<int> { 400, 800, 1200 }, financeManager.costUpgradeAsteroidMine, "CostUpgradeAsteroidMine should match.");
            Assert.AreEqual(4000, financeManager.costBuildShipyard, "CostBuildShipyard should be 4000.");
        }

        [TestMethod]
        public void LoadCardsConfig_ValidFile_ReturnsCorrectCardsList()
        {
            var cards = ConfigLoader.LoadCardConfig(TestValidCardsConfigPath);
            Assert.AreEqual(cards.Count(), 3, "Cards length should be 3");

            Assert.AreEqual(cards[0].Title, "Atak piratów", "First card title should be 'Atak piratów'");
            Assert.AreEqual(cards[1].Title, "Obrona przed piratami", "Second card title should be 'Obrona przed piratami'");
            Assert.AreEqual(cards[2].Title, "Bilet galaktyczny", "Third card title should be 'Bilet galaktyczny'");

            Assert.AreEqual(cards[0].Description, "Zostałeś zaatakowany przez piratów.\nJeżeli nie zapłacisz 800 kredytów okupu stracisz 2 kolejki.");
            Assert.AreEqual(cards[1].Description, "Strażnicy galaktyki zgodzili sie obronić cię przed jednym atakiem piratów. Wezwij ich kiedy tylko chcesz.");
            Assert.AreEqual(cards[2].Description, "Możesz przenieść się do dowolnej stacji kolei galaktycznych.\n");

            Assert.AreEqual(cards[0].strategies.Count(), 3, "Card should have 3 strategy");
            Assert.IsFalse(cards[0].ApplyTogether, "Card should have apply_strategy set to false");

            Assert.AreEqual(cards[1].strategies.Count(), 1, "Card should have 1 strategy");
            Assert.IsTrue(cards[1].ApplyTogether, "Card should have apply_strategy set to True");

            Assert.AreEqual(cards[2].strategies.Count(), 2, "Card should have 2 strategy");
            Assert.IsFalse(cards[2].ApplyTogether, "Card should have apply_strategy set to false");

        }

        [TestMethod]
        public void LoadGalaxyConfig_ValidFile_ReturnsCorrectEntitiesList()
        {
            var (entities, systems) = ConfigLoader.LoadGalaxyConfig(TestValidGalaxyConfigPath);
            Assert.AreEqual(systems.Count(), 3, "Systems count should be 3");
            Assert.AreEqual(entities.Count(), 7, "Entities count should be 7");

            Assert.IsTrue(entities[0] is Station);
            Assert.IsTrue(entities[1] is HabitablePlanet);
            Assert.IsTrue(entities[2] is HabitablePlanet);
            Assert.IsTrue(entities[3] is Station);
            Assert.IsTrue(entities[4] is Singularity);
            Assert.IsTrue(entities[5] is PiratePlanet);
            Assert.IsTrue(entities[6] is HabitablePlanet);

            Assert.AreEqual("Hyperion Gate", entities[0].Name);
            Assert.AreEqual("Aureon", entities[1].Name);
            Assert.AreEqual("Zenthor", entities[2].Name);
            Assert.AreEqual("Nebula Nexus", entities[3].Name);
            Assert.AreEqual("Osobliwość", entities[4].Name);
            Assert.AreEqual("Planeta Piratów", entities[5].Name);
            Assert.AreEqual("Zagw", entities[6].Name);

        }

        [TestMethod]
        public void LoadCardConfig_NonExistentFile_ReturnsEmptyList()
        {
            List<Card> cards = ConfigLoader.LoadCardConfig(TestNonExistentConfigPath);
            Assert.IsNotNull(cards, "List of cards should not be null.");
            Assert.AreEqual(0, cards.Count, "Cards list should be empty for a non-existent file.");
        }

        [TestMethod]
        public void LoadGalaxyConfig_NonExistentFile_ReturnsEmptyList()
        {
            var (entities, systems) = ConfigLoader.LoadGalaxyConfig(TestNonExistentConfigPath);
            Assert.IsNotNull(entities, "List of entities should not be null.");
            Assert.IsNotNull(systems, "List of systems should not be null.");
            Assert.AreEqual(0, entities.Count, "Entities list should be empty for a non-existent file.");
            Assert.AreEqual(0, systems.Count, "Systems list should be empty for a non-existent file.");
        }

        [TestMethod]
        public void LoadFinancesConfig_NonExistentFile_ReturnsNull()
        {
            var manager = ConfigLoader.LoadFinanceManagerConfig(TestNonExistentConfigPath);
            Assert.IsNull(manager);
        }

        [TestMethod]
        public void LoadFinancesConfig_InvalidFormat_ReturnsNull()
        {
            var manager = ConfigLoader.LoadFinanceManagerConfig(TestInValidFinancesConfigPath);
            Assert.IsNull(manager);
        }

        [TestMethod]
        public void LoadCardConfig_InvalidJsonFormat_ReturnsEmptyList()
        {
            List<Card> cards = ConfigLoader.LoadCardConfig(TestInValidCardsConfigPath);
            Assert.IsNotNull(cards, "List of cards should not be null.");
            Assert.AreEqual(0, cards.Count, "Cards list should be empty for invalid JSON format.");
        }

        [TestMethod]
        public void LoadGalaxyConfig_InvalidJsonFormat_ReturnsEmptyList()
        {
            var (entities, systems) = ConfigLoader.LoadGalaxyConfig(TestNonExistentConfigPath);
            Assert.IsNotNull(entities, "List of entities should not be null.");
            Assert.IsNotNull(systems, "List of systems should not be null.");
            Assert.AreEqual(0, entities.Count, "Entities list should be empty.");
            Assert.AreEqual(0, systems.Count, "Systems list should be empty.");
        }
    }
}
