using System;
using UnityEngine;

namespace UnityUtils.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class RequiredAttribute : PropertyAttribute
    {
        public string Message = String.Empty;
        public RequiredAttribute(string message) => Message = message;
        
    }
}