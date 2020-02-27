const LinkedList = require("../../../server/classes/LinkedList");
const should = require('should');

function matches(array, ll) {
    for (let i of array)
        i.should.be.equal(ll.dequeue());

    ll.isEmpty().should.be.true();
}

function randomArray(len, max = 100) {
    if (len < 0)
        len = 0;

    let ret = [];

    for (let i = 0; i < len; i++)
        ret.push(Math.floor(Math.random() * max));

    return ret;
}

function verify(arr) {
    matches(arr, new LinkedList(arr));
}

describe('Linked list', function () {
    this.slow(5);
    it('Empty linked list has valid string', function () {
        verify([]);
    });

    describe('Pushing', function () {
        this.slow(100);
        it('Testing construction/pushing', function () {
            for (let i = 1; i <= 100; i++)
                verify(randomArray(i));

            return;
        });
    });

    describe('Deleting', function () {
        it('testing deletion', function () {
            let arr = [];

            for (let i = 0; i < 10; i++)
                arr.push(i);

            for (let i = 0; i < arr.length; i++) {
                let ll = new LinkedList(arr);
                let it = ll.it();

                for (let j = 0; j < i; j++)
                    it.next();

                it.delete();

                for (let j = 0; j < arr.length; j++) {
                    if (j == i)
                        continue;

                    ll.dequeue().should.equal(arr[j]);
                }

                return ll.isEmpty().should.be.true();
            }
        });
    });
});
