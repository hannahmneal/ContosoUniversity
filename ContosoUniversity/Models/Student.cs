using System;
using System.Collections.Generic;
//NOTE: This enables annotations, which we need for displaying specific data:
using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstMidName { get; set; }

        //NOTE: This annotations below (in brackets) will enable the datetime format to be shown as YYYY-MM-DD instead of YYYY-MM-DD 00:00:00:
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public DateTime EnrollmentDate { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }
    }
}