using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.ValueObjects
{
    public sealed class Email : IEquatable<Email>
    {
        public string Value { get; set; }
        public Email(string value)
        {
            Value = value;
        }

        public static Email Create(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Email Can not be empty");

            var email = input.Trim().ToLower();

            if (!email.Contains("@"))
            {
                throw new ArgumentException("invalid email address");
            }

            return new Email(email);
        }

        public bool Equals(Email? other)
        {
            return other is not null && Value == other.Value;
        }

        public override bool Equals(object? obj)
        {
            return obj is Email other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString() => Value;

    }
}
