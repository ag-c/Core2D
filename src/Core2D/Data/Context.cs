﻿using System.Collections.Generic;
using System.Collections.Immutable;

namespace Core2D.Data
{
    /// <summary>
    /// Data context.
    /// </summary>
    public class Context : ObservableObject, IContext
    {
        private ImmutableArray<IProperty> _properties;
        private IRecord _record;

        /// <inheritdoc/>
        public ImmutableArray<IProperty> Properties
        {
            get => _properties;
            set => Update(ref _properties, value);
        }

        /// <inheritdoc/>
        public IRecord Record
        {
            get => _record;
            set => Update(ref _record, value);
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            var properties = this._properties.Copy(shared).ToImmutable();

            return new Context()
            {
                Name = this.Name,
                Properties = properties,
                Record = (IRecord)this.Record.Copy(shared)
            };
        }

        /// <summary>
        /// Check whether the <see cref="Properties"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeProperties() => true;

        /// <summary>
        /// Check whether the <see cref="Record"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeRecord() => _record != null;
    }
}
