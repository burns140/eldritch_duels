const fs = require('fs');
const rp = require('request-promise');
const $ = require('cheerio');
const npcUrl = 'https://bloodborne.wiki.fextralife.com/NPCs'
const baseUrl = `https://bloodborne.wiki.fextralife.com`

var npcLink = [];
var npcs = [];

async function main() {
    await getArrs();
    writeArrs();
}

async function getArrs() {
    await rp(npcUrl).then(html => {
        var chars = $('.includePageList', html).text();
        $('.includePageListPageUrl', html).each( (index, val) => {
            var link = $(val).attr('href');
            npcLink.push(`${baseUrl}${link}`);
        });
        var tempArr = chars.split('\n');
        npcs = tempArr;
    });
    var i = 0;
    for (npc of npcs) {
        npcs[i] = npc.trim();
        i++;
    }
    npcs = npcs.filter(Boolean);
    // console.log(npcs);
}

function writeArrs() {
    var npcfile = fs.createWriteStream('npcs.txt');
    npcfile.write('NPCS\n--------------------------\n');
    npcs.forEach( (val, key, map) => {
        npcfile.write(`${val}\n`);
    });
    npcfile.write('\n\nLINKS\n--------------------------\n');
    npcLink.forEach( (val, key, mapp) => {
        npcfile.write(`${val}\n`);
    });
    npcfile.end();
}

main();
