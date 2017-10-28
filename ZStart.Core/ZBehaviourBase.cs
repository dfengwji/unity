﻿using System;
using UnityEngine;

namespace ZStart.Core
{
    public abstract class ZBehaviourBase : MonoBehaviour, IDisposable
    {
        private Transform _mTransform;
        public Transform mTransform
        {
            get
            {
                if (_mTransform == null)
                    _mTransform = this.transform;
                return _mTransform;
            }
        }

        protected bool _isStartEnd = false;
        public bool isStartEnd
        {
            get { return _isStartEnd; }
        }
        protected bool _disposed = false;
        public void Dispose()
        {
            Dispose(true);
            //System.GC.SuppressFinalize(this);
        }

        public virtual void TestData()
        {

        }

        protected virtual void Dispose(bool disposing)
        {

        }
    }
}
