using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SharedTypes.Dtos;

namespace SharedTypes.Entities
{
    public class UserEntity
    {
        [Key] public int Id { get; set; }

        [Required, MaxLength(100)] public string Login { get; set; }

        [Required, MinLength(64), MaxLength(64)]
        public string Password { get; set; }

        public ICollection<ShoppingListEntity> ShoppingListEntities { get; set; } = new List<ShoppingListEntity>();
    }
}