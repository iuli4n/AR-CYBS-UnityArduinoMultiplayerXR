using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

    // *** OUTDATED DESCRIPTION FROM MODELNETWORKING:
    // This is a datatype that functions as a list, and can be serialized in a networked model
    // USAGE: The onListChanged event is listened to by the parent model, and when invoked it serializes this along with the parent model
    // TODO: accessors should check if anything chagned, before raising event
    // TODO: clean up old code; including hasChangedSinceWrite
    public class NetworkedList<T> : IList<T> //, NetworkedModelBase //where T : IConvertible
    {
        private List<T> mylist;

        //private NetworkedModel parentModel;

        // Event called when the list is changed, either locally or through deserialization
        // Note, this should really only be used by the host NetworkedModel, not by the public
        public event ListChanged onListChanged;
        public delegate void ListChanged(bool isLocalChange);


        //private bool hasChangedSinceWrite = false;

        public NetworkedList()//NetworkedModel parentModel)
        {
            mylist = new List<T>();
            //this.parentModel = parentModel;
            onListChanged += (x) => { /* if (x == true) hasChangedSinceWrite = true;*/ };
        }


        /*** TODO: TO REDUCE LAG, SHOULD USE RPCS ON UPDATE ******/

        

        public void Do_OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            // called both when reading or writing

            if (stream.IsWriting)
            {
                // WRITING: we own this player so we send out this data : send the others our data

                // TODOIR: NEXT: maybe when this is false the reading fails ??
                //if (!hasChangedSinceWrite) return;

                Debug.Log("** LIST WRITE " + mylist.Count);

                stream.SendNext(mylist.Count);
                foreach (T e in this)
                {
                    Debug.Log(e);
                    stream.SendNext(e);
                }

                //hasChangedSinceWrite = false;
            }
            else
            {
                // READING from network, receive data from another client
                mylist.Clear();
                int count = (int)stream.ReceiveNext();

                Debug.Log("*** LIST READ " + count);

                for (int i = 0; i < count; i++)
                {
                    T e = (T)stream.ReceiveNext();
                    mylist.Add(e);
                }
                onListChanged?.Invoke(false);

            }

        }



        /***
        public void SerializeModelFull(Stream outStream, NetworkedModelFieldSerializer formatter)
        {
            formatter.Serialize(outStream, NetworkedModelMessagePadding.ModelFullStart);

            formatter.Serialize(outStream, mylist.Count);
            foreach (T e in this)
            {
                formatter.Serialize(outStream, e);
            }

            formatter.Serialize(outStream, NetworkedModelMessagePadding.ModelFullEnd);
        }

        public void DeserializeModelFull(Stream inStream, NetworkedModelFieldSerializer formatter)
        {
            Debug.Assert(NetworkedModelMessagePadding.ModelFullStart == (NetworkedModelMessagePadding)(int)formatter.Deserialize(inStream));

            mylist.Clear();
            int count = (int)formatter.Deserialize(inStream);
            for (int i = 0; i < count; i++)
            {
                T e = (T)formatter.Deserialize(inStream);
                mylist.Add(e);
            }
            onListChanged?.Invoke(false); // note, this assumes list has actually changed; but it wouldn't be updated otherwise, i think

            Debug.Assert(NetworkedModelMessagePadding.ModelFullEnd == (NetworkedModelMessagePadding)(int)formatter.Deserialize(inStream));

        }
        ***/







        public int Count => ((IList<T>)mylist).Count;

        public bool IsReadOnly => ((IList<T>)mylist).IsReadOnly;

        public T this[int index]
        {
            get { return ((IList<T>)mylist)[index]; }
            set
            {
                if (!((IList<T>)mylist)[index].Equals(value))
                {
                    ((IList<T>)mylist)[index] = value;
                    onListChanged?.Invoke(true);
                }
            }
        }

        public int IndexOf(T item)
        {
            return ((IList<T>)mylist).IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            ((IList<T>)mylist).Insert(index, item);
            onListChanged?.Invoke(true);
        }

        public void RemoveAt(int index)
        {
            ((IList<T>)mylist).RemoveAt(index);
            onListChanged?.Invoke(true);
        }

        public void Add(T item)
        {
            ((IList<T>)mylist).Add(item);
            onListChanged?.Invoke(true);
        }

        public void Clear()
        {
            if (mylist.Count > 0)
            {
                ((IList<T>)mylist).Clear();
                onListChanged?.Invoke(true);
            }
        }

        public bool Contains(T item)
        {
            return ((IList<T>)mylist).Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ((IList<T>)mylist).CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            bool x = ((IList<T>)mylist).Remove(item);
            if (x) onListChanged?.Invoke(true);
            return x;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IList<T>)mylist).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<T>)mylist).GetEnumerator();
        }
    }
