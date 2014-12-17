namespace Ix.Palantir.Pooling.Storage
{
    using System;
    using System.Collections.Generic;

    public class CircularStore<T> : IItemStore<T>
    {
        private readonly List<Slot<T>> slots;
        private int freeSlotCount;
        private int position = -1;

        public CircularStore(int capacity)
        {
            this.slots = new List<Slot<T>>(capacity);
        }

        public int Count
        {
            get
            {
                return this.freeSlotCount;
            }
        }

        public T Fetch()
        {
            if (this.Count == 0)
            {
                throw new InvalidOperationException("The buffer is empty.");
            }

            int startPosition = this.position;

            do
            {
                Slot<T> slot = this.slots[this.position];
                
                if (!slot.IsInUse)
                {
                    slot.IsInUse = true;
                    --this.freeSlotCount;
                    this.Advance();
                    return slot.Item;
                }

                this.Advance();
            } 
            while (startPosition != this.position);

            throw new InvalidOperationException("No free slots.");
        }
        public T Peek()
        {
            if (this.Count == 0)
            {
                return default(T);
            }

            int startPosition = this.position;

            do
            {
                Slot<T> slot = this.slots[this.position];

                if (!slot.IsInUse)
                {
                    return slot.Item;
                }

                this.Advance();
            }
            while (startPosition != this.position);

            return default(T);
        }
        public void Store(T item)
        {
            Slot<T> slot = this.slots.Find(s => object.Equals(s.Item, item));

            if (slot == null)
            {
                slot = new Slot<T>(item);
                this.slots.Add(slot);
            }

            slot.IsInUse = false;
            ++this.freeSlotCount;
        }

        private void Advance()
        {
            this.position = (this.position + 1) % this.slots.Count;
        }
    }
}