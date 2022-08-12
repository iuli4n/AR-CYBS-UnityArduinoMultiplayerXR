using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationValueModel : MonoBehaviour
{
        public delegate void UpdateRotVal();
        public static event UpdateRotVal onRotValUpdate;

        private int rotVal;

        public int RotVal
        {
            get
            {
                return rotVal;
            }

            set
            {
                if (RotVal != value)
                {
                    rotVal = value;

                    if (onRotValUpdate != null)
                    {
                        onRotValUpdate();
                    }
                }

            }
        }
}
