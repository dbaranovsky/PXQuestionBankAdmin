using System;
using System.Collections.Generic;
using System.Linq;
using System.Dynamic;
using System.Linq.Expressions;
using System.ComponentModel;

namespace Bfw.Common.DynamicExtention
{
   
    public class ElasticObject : DynamicObject, IElasticHierarchyWrapper, INotifyPropertyChanged
    {

        #region Private
        private readonly IElasticHierarchyWrapper elasticProvider = new SimpleHierarchyWrapper();
        private NodeType nodeType = NodeType.Element;
        #endregion

        #region Constructor

        /// <summary>
		/// Create Elastic Object
        /// </summary>
        public ElasticObject()
        {
            InternalName = "id" + Guid.NewGuid().ToString();
        }


        /// <summary>
		/// Create Elastic Object
        /// </summary>
        /// <param name="name"></param>
        public ElasticObject(string name)
        {
            InternalName = name;
        }

        internal ElasticObject(string name, object value) : this(name)
        {
            InternalValue = value;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add a member to this element, with the specified value
        /// </summary>
        /// <param name="memberName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal ElasticObject CreateOrGetAttribute(string memberName, object value)
        {
            if (!HasAttribute(memberName))
            {
                AddAttribute(memberName, new ElasticObject(memberName,value));
            }

            return Attribute(memberName);

        }

        /// <summary>
        /// Fully qualified name
        /// </summary>
        public string InternalFullName
        {
            get
            {
                string path = InternalName;
                var parent = InternalParent;

                while (parent != null)
                {
                    path = parent.InternalName + "_" + path;
                    parent = parent.InternalParent;
                }

                return path;
            }
        }



        /// <summary>
        /// Interpret a method call
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="args"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryInvokeMember(System.Dynamic.InvokeMemberBinder binder, object[] args, out object result)
        {
            var obj = new ElasticObject(binder.Name,null);
            AddElement(obj);
            result = obj;
            return true;

        }


        /// <summary>
        /// Interpret the invocation of a binary operation
        /// </summary>
        public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result)
        {

            if (binder.Operation == ExpressionType.LeftShiftAssign && nodeType==NodeType.Element)
            {
                InternalContent = arg;
                result = this;
                return true;
            }

            if (binder.Operation == ExpressionType.LeftShiftAssign  && nodeType == NodeType.Attribute)
            {
                InternalValue = arg;
                result = this;
                return true;
            }

            switch (binder.Operation)
            {
            	case ExpressionType.LeftShift:
            		if (arg is string)
            		{
            			var exp = new ElasticObject(arg as string, null) { nodeType = NodeType.Element };
            			AddElement(exp);
            			result = exp;
            			return true;
            		}
            		if (arg is ElasticObject)
            		{
            			var eobj = arg as ElasticObject;
            			if (!Elements.Contains(eobj))
            				AddElement(eobj);
            			result = eobj;
            			return true;
            		}
            		break;
            	case ExpressionType.LessThan:
            		{
            			string memberName = arg as string;
            			if (arg is string)
            			{
            				if (!HasAttribute(memberName))
            				{
            					var att = new ElasticObject(memberName, null);
            					AddAttribute(memberName, att);
            					result = att;
            					return true;
            				}
            				throw new InvalidOperationException("An attribute with name" + memberName +  " already exists");
            			}

            			if (arg is ElasticObject)
            			{
            				var eobj = arg as ElasticObject;
            				AddAttribute(memberName, eobj);
            				result = eobj;
            				return true;
            			}
            		}
            		break;
            }

            return base.TryBinaryOperation(binder, arg, out result);
        }


        /// <summary>
        /// Try the unary operation.
        /// </summary>
        public override bool TryUnaryOperation(UnaryOperationBinder binder, out object result)
        {
            if (binder.Operation == ExpressionType.OnesComplement)
            {
                result = (nodeType == NodeType.Element) ? InternalContent : InternalValue;
                return true;
            }

            if (binder.Operation == ExpressionType.Not)
            {
                result = this.ToXElement();
                return true;
            }


            return base.TryUnaryOperation(binder, out result);
        }


        /// <summary>
        /// Handle the indexer operations
        /// </summary>
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            result = null;

            if ((indexes.Length == 1) && indexes[0] == null)
            {
                result = elasticProvider.Elements.ToList();
            }
            else if ((indexes.Length == 1) && indexes[0] is int)
            {
                var indx = (int)indexes[0];
                var elmt = Elements.ElementAt(indx);
                result = elmt;
                
            }
            else if ((indexes.Length == 1) && indexes[0] is Func<dynamic,bool>)
            {
                var filter = indexes[0] as Func<dynamic, bool>;
                result = Elements.Where
                   (c => filter(c) ).ToList();
            }
            else
            {
                result = Elements.Where
                    (c => indexes.Cast<string>().Contains(c.InternalName)).ToList();
            }

            return true;
        }



        /// <summary>
        /// Catch a get member invocation
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {

            if (ProcessSpecialDirective(binder.Name, out result))
                return true;


            if (elasticProvider.HasAttribute(binder.Name))
            {
                result = elasticProvider.Attribute(binder.Name).InternalValue;
            }
            else
            {
                var obj = elasticProvider.Element(binder.Name);
                if (obj != null)
                {
                    result = obj;
                }
                else
                {
                    var exp = new ElasticObject(binder.Name,null);
                    elasticProvider.AddElement(exp);
                    result = exp;
                }
            }

            return true;
        }


        /// <summary>
        /// Catch a set member invocation
        /// </summary>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var memberName = binder.Name;

            if (value is ElasticObject)
            {
                var eobj = value as ElasticObject;
                if (!Elements.Contains(eobj))
                    AddElement(eobj);
            }
            else
            {
                if (!elasticProvider.HasAttribute(memberName))
                {
                    elasticProvider.AddAttribute(memberName, new ElasticObject(memberName,value));
                }
                else
                {
                    elasticProvider.SetAttributeValue(memberName,value);
                }
            }

            OnPropertyChanged(memberName);

            return true;
        }


        /// <summary>
        /// Check for any specific process directives
        /// </summary>
        /// <param name="membername"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        bool ProcessSpecialDirective(string membername, out object result)
        {
            var sdirectives = Enum.GetNames(typeof(SpecialDirective));
            foreach (var d in sdirectives)
            {
                if (membername.EndsWith(d))
                {
                    var directive = (SpecialDirective)Enum.Parse(typeof(SpecialDirective), d);

                    if (directive == SpecialDirective._All)
                    {
                        string elementName = membername.Substring(0,membername.Length-d.Length);
                        result = Elements.Where(e => e.InternalName == elementName).ToList();
                        return true;
                    }
                }
            }

            result = null;
            return false;
        }

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
		/// PropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string prop)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        #endregion

        #region IElasticHierarchyWrapper<ElasticObject> Members

        public IEnumerable<KeyValuePair<string, ElasticObject>> Attributes
        {
            get { return elasticProvider.Attributes;  }
        }

        public bool HasAttribute(string name)
        {
            return elasticProvider.HasAttribute(name);
        }

        public IEnumerable<ElasticObject> Elements
        {
            get { return elasticProvider.Elements; }
        }

        public void SetAttributeValue(string name, object obj)
        {
            elasticProvider.SetAttributeValue(name, obj);
        }

        public object GetAttributeValue(string name)
        {
            return elasticProvider.GetAttributeValue(name);
        }

        public ElasticObject Attribute(string name)
        {
            return elasticProvider.Attribute(name);
        }

        public ElasticObject Element(string name)
        {
            return elasticProvider.Element(name);
        }

        public void AddAttribute(string key, ElasticObject value)
        {
            value.nodeType = NodeType.Attribute;
            value.InternalParent = this;
            elasticProvider.AddAttribute(key, value);
        }

        public void RemoveAttribute(string key)
        {
            elasticProvider.RemoveAttribute(key);
        }

        public void AddElement(ElasticObject element)
        {
            element.nodeType = NodeType.Element;
            element.InternalParent = this;
            elasticProvider.AddElement(element);
        }

        public void RemoveElement(ElasticObject element)
        {
            elasticProvider.RemoveElement(element);
        }

        public object InternalValue
        {
            get
            {
                return elasticProvider.InternalValue;
            }
            set
            {
                elasticProvider.InternalValue = value;
            }
        }

        public object InternalContent
        {
            get
            {
                return elasticProvider.InternalContent;
            }
            set
            {
                elasticProvider.InternalContent = value;
            }
        }

        public string InternalName
        {
            get
            {
                return elasticProvider.InternalName;
            }
            set
            {
                elasticProvider.InternalName = value;
            }
        }

        public ElasticObject InternalParent
        {
            get
            {
                return elasticProvider.InternalParent;
            }
            set
            {
                elasticProvider.InternalParent = value;
            }
        }

        #endregion

        
      
    }


    /// <summary>
    /// Special directives for filtering option
    /// </summary>
    public enum SpecialDirective
    {
        _All,
    }

    public enum NodeType
    {
        Element,
        Attribute
    }
}
