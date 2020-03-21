let _ = require("lodash");

/**@template T */
class LinkedList {
    /**
     * @constructor
     * @param {T | T[]} [stuff]
    */
    constructor(stuff) {
        this.head = /** @type {LinkedListNode<T>} */ (new LinkedListNode());
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

    /** @returns {LLIterator<T>} */
    iterator() {
        return new LLIterator(this);
    }

    it() {
        return this.iterator();
    }
    
    /**
     * puts value on end of queue
     * @param {T} val
     */
    push(val) {
        new LinkedListNode(val, this.head, this.head.prev);
        this._size++;
        return this;
    }
    
    /**
     * puts value on end of queue
     * @param {T} val
     */
    enqueue(val) {
        return this.push(val);
    }

    /** @returns {T | undefined} */
    peekFirst() {
        return this.isEmpty() ? undefined : this.node.next.value;
    }

    /** @returns {T} */
    peekLast() {
        return this.isEmpty() ? undefined : this.node.prev.value;
    }
    
    /**
     * takes first value on queue
     * @returns {T}
     */
    dequeue() {
        let node = (this.node.next);
        if (node == this.head)
            return undefined;

        this.node.next = node.next;
        node.next.prev = this.node;
        this._size--;
        return node.value;
    }

    isEmpty() {
        return this.node.next == this.head;
    }

    /** @returns {string} */
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

/**@template T */
class LinkedListNode {
    /**
     * 
     * @param {T} value
     * @param {LinkedListNode<T>} next
     * @param {LinkedListNode<T>} prev
     */
    constructor(value, next = this, prev = this) {
        this.value = value;
        this.next = next;
        this.prev = prev;
        
        next.prev = this;
        prev.next = this;
    }
}

/** @template T */
class LLIterator {
    /** @param {LinkedList<T>} ll*/
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

    /**
     * Sets or gets the value at current node
     * @param {T} [val] - if defined, sets the value to it. Otherwise, gets the value
     * @returns {T}
     */
    value(val) {
        if (val == undefined)
            return this.node.value;
        else
            return this.node.value = val;
    }

    /**
     * Inserts value after current node
     * @param {T} val
     */
    insertAfter(val) {
        if (this.node == this.list.head)
            throw Error("inserting after end of list");

        this._size++;
        new LinkedListNode(val, this.node.next, this.node);
    }

    /**
     * Deletes current node and moves to next one
     * @returns {T} The value at the current node
    */
    delete() {
        if (this.node == this.list.head)
            return undefined;

        let node = this.node;

        node.next.prev = this.node.prev;
        node.prev.next = this.node.next;

        this.node = this.node.next;
        this.list._size--;
        return node.value;
    }
}

module.exports = LinkedList;