const TYPE_CREATURE = "Creature";
const TYPE_SPELL = "Spell";
const COMMON = "Common";
const RARE = "Rare";
const LEGENDARY = "Legendary";

const defaultPath =  "client/Eldritch Duels/Assets/images";

const cards = [
    {
        Name: "Test 0",
        ID: 0,
        Cost: 10,
        Power: 10,
        Defense: 100,
        Type: TYPE_CREATURE,
        Rarity: LEGENDARY,
        Image: "TestCard0"
    },

    {
        Name: "Test 1",
        ID: 1,
        Cost: 5,
        Power: 0,
        Defense: 0,
        Type: TYPE_SPELL,
        Rarity: COMMON,
        Image: "TestCard1"
    },

    {
        Name: "Mi_Go",
        ID: 2,
        Cost: 1,
        Power: 100,
        Defense: 200,
        Type: TYPE_CREATURE,
        Rarity: COMMON,
        Image: "MiGo"
    },

    {
        Name: "Mi_Go Worker",
        ID: 3,
        Cost: 2,
        Power: 200,
        Defense: 200,
        Type: TYPE_CREATURE,
        Rarity: RARE,
        Image: "Migo Worker"
    },

    {
        Name: "Mi_Go Queen",
        ID: 4,
        Cost: 5,
        Power: 500,
        Defense: 500,
        Type: TYPE_CREATURE,
        Rarity: LEGENDARY,
        Image: "Migo Queen"
    },

    {
        Name: "Beast Patient",
        ID: 5,
        Cost: 5,
        Power: 5,
        Defense: 5,
        Type: TYPE_CREATURE,
        Rarity: COMMON,
        Image: "BeastPatient"
    },

    {
        Name: "Chime Maiden",
        ID: 6,
        Cost: 6,
        Power: 6,
        Defense: 6,
        Type: TYPE_CREATURE,
        Rarity: RARE,
        Image: "ChimeMaiden"
    },

    {
        Name: "Brain of Mensis",
        ID: 7,
        Cost: 7,
        Power: 7,
        Defense: 7,
        Type: TYPE_CREATURE,
        Rarity: RARE,
        Image: "BrainOfMensis"
    },

    {
        Name: "Mi-Go Zombie",
        ID: 8,
        Cost: 8,
        Power: 8,
        Defense: 8,
        Type: TYPE_CREATURE,
        Rarity: COMMON,
        Image: "Mi-GoZombie"
    },

    {
        Name: "Snatcher",
        ID: 9,
        Cost: 9,
        Power: 9,
        Defense: 9,
        Type: TYPE_CREATURE,
        Rarity: RARE,
        Image: "Snatcher"
    },

    {
        Name: "Nightmare Apostle",
        ID: 10,
        Cost: 10,
        Power: 10,
        Defense: 10,
        Type: TYPE_CREATURE,
        Rarity: COMMON,
        Image: "NightmareApostle"
    },

    {
        Name: "Quicksilver Bullets",
        ID: 11,
        Cost: 11,
        Power: null,
        Defense: null,
        Type: TYPE_SPELL,
        Rarity: RARE,
        Image: "QuicksilverBullets"
    },

    {
        Name: "Great One's Wisdom",
        ID: 12,
        Cost: 12,
        Power: null,
        Defense: null,
        Type: TYPE_SPELL,
        Rarity: RARE,
        Image: "GreatOnesWisdom"
    },

    {
        Name: "Blood Starved Beast",
        ID: 13,
        Cost: 13,
        Power: 13,
        Defense: 13,
        Type: TYPE_CREATURE,
        Rarity: LEGENDARY,
        Image: "BloodStarvedBeast"
    },
    
    {
        Name: "Moon Presence",
        ID: 14,
        Cost: 14,
        Power: 14,
        Defense: 14,
        Type: TYPE_CREATURE,
        Rarity: LEGENDARY,
        Image: "MoonPresence"
    },

    {
        Name: "Ludwig, Holy Blade",
        ID: 15,
        Cost: 15,
        Power: 15,
        Defense: 15,
        Type: TYPE_CREATURE,
        Rarity: LEGENDARY,
        Image: "LudwigHolyBlade"
    },

    {
        Name: "Blood Vial",
        ID: 16,
        Cost: 16,
        Power: null,
        Defense: null,
        Type: TYPE_SPELL,
        Rarity: COMMON,
        Image: "BloodVial"
    },

    {
        Name: "Lady Maria",
        ID: 17,
        Cost: 17,
        Power: 17,
        Defense: 17,
        Type: TYPE_CREATURE,
        Rarity: LEGENDARY,
        Image: "LadyMaria"
    },

    {
        Name: "Pungent Blood Cocktail",
        ID: 18,
        Cost: 18,
        Power: null,
        Defense: null,
        Type: TYPE_SPELL,
        Rarity: RARE,
        Image: "PungentBloodCocktail"
    },

    {
        Name: "Madman's Knowledge",
        ID: 19,
        Cost: 19,
        Power: null,
        Defense: null,
        Type: TYPE_SPELL,
        Rarity: RARE,
        Image: "MadmansKnowledge"
    }
];