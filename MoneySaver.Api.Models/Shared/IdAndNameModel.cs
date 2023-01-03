using System.Collections.Generic;

namespace MoneySaver.Api.Models.Shared
{
    public class IdAndNameModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            IdAndNameModel other = (IdAndNameModel)obj;
            return other.Id == this.Id && other.Name == this.Name;
        }

        public override int GetHashCode()
        {
            int hashCode = -1919740922;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            return hashCode;
        }
    }
}
