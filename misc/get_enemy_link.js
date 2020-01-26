const fs = require('fs');
const rp = require('request-promise');
const $ = require('cheerio');
const enemyUrl = `https://bloodborne.wiki.fextralife.com/Enemies`
const baseUrl = `https://bloodborne.wiki.fextralife.com`

var enemyLink = [];
var enemies = [];

async function main() {
    await getArrs();
    writeArrs();
}

async function getArrs() {
    await rp(enemyUrl).then(html => {
        $('h3', html).each( (index, val) => {
            var tv = $(val).text();
            $('.wiki_link', val).each( (ind, value) => {
                var link = $(value).attr('href');
                enemyLink.push(`${baseUrl}${link}`)
            });
            if (!(tv.includes('\t'))) {
                enemies.push(tv.trim());
            };
        })
    })
}

function writeArrs() {
    var enemyfile = fs.createWriteStream('enemies.txt');
    enemyfile.write('ENEMIES\n--------------------------\n');
    enemies.forEach( (val, key, map) => {
        enemyfile.write(`${val}\n`);
    });
    enemyfile.write('\n\nLINKS\n--------------------------\n');
    enemyLink.forEach( (val, key, map) => {
        enemyfile.write(`${val}\n`);
    });
    enemyfile.end();
}

main();