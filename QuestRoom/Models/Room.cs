﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QuestRoom.Models
{
    public class Room
    {
        public int Id { get; set; }
        [Required]
        [StringLength(60, MinimumLength = 3)]
        public string Name { get; set; }
        [Required]
        [StringLength(500, MinimumLength = 20)]
        public string Description { get; set; }
        public int TimeOfPassing { get; set; }
        [Required]
        public int MinAmountOfPlayers { get; set; }
        public int MaxAmountOfPlayers { get; set; }
        public int MinAge { get; set; }
        [Required]
        [StringLength(60, MinimumLength = 3)]
        public string Address { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [RegularExpression("(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\x01 -\x08\x0b\x0c\x0e -\x1f\x21\x23 -\x5b\x5d -\x7f] |\\[\x01-\x09\x0b\x0c\x0e-\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\\])")]
        public string Email { get; set; }
        public string Company { get; set; }
        [Range(1, 5)]
        public float Rating { get; set; }
        public int LevelOfFear { get; set; }
        [Range(1, 5)]
        public int LevelOfDifficulty { get; set; }
    }
}