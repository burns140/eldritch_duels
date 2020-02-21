let _ = require("lodash");

module.exports = class LinkedList {
    constructor(stuff) {
        this.head = new LinkedListNode();
        this.node = this.head;
        this._size = 0;

        if (_.isUndefined(stuff))
            return;

        if (!_.isArray(stuff))
            stuff = [stuff];

        for (let i of stuff)
            this.push(i);
    }

    size() { return this._size; }

    iterator() {
        return new LLIterator(this);
    }

    it() {
        return this.iterator();
    }

    // puts value on end of queue
    push(val) {
        new LinkedListNode(val, this.head, this.head.prev);
        this._size++;
        return this;
    }

    // puts value on end of queue
    enqueue(val) {
        return this.push(val);
    }

    peekFirst() {
        return this.isEmpty() ? undefined : this.node.next.value;
    }

    peekLast() {
        return this.isEmpty() ? undefined : this.node.prev.value;
    }

    // takes first value on queue
    dequeue() {
        let node = this.node.next;
        if (node == this.head)
            return null;

        this.node.next = node.next;
        node.next.prev = this.node;
        this._size--;
        return node.value;
    }

    isEmpty() {
        return this.node.next == this.head;
    }

    toString() {
        let it = this.iterator();
        if (!it.hasNext())
            return "[]";

        let ret = "[ " + it.value();

        for (it.next(); it.hasValue(); it.next())
            ret += ", " + it.value();

        ret += " ]";
        return ret;
    }
}

class LinkedListNode {
    constructor(value, next = this, prev = this) {
        this.value = value;
        this.next = next;
        this.prev = prev;
        
        next.prev = this;
        prev.next = this;
    }
}

class LLIterator {
    constructor(ll) {
        this.list = ll;
        this.node = ll.node.next;
    }

    hasNext() {
        return this.node.next != this.list.head;
    }

    hasValue() {
        return this.node != this.list.head;
    }

    next() {
        if (this.node == this.list.head)
            return;
        
        this.node = this.node.next;;
    }

    value(val) {
        if (val == undefined)
            return this.node.value;
        else
            this.node.value = val;
    }

    insertAfter(val) {
        if (this.node == this.list.head)
            throw Error("inserting after end of list");

        this._size++;
        new LinkedListNode(val, this.node.next, this.node);
    }

    delete() {
        if (this.node == this.list.head)
            return;

        let node = this.node;

        node.next.prev = this.node.prev;
        node.prev.next = this.node.next;

        this.node = this.node.next;
        this.list._size--;
        return node.value;
    }
}