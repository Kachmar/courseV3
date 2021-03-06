﻿using System;
using System.Collections.Generic;

namespace University.MVC.ViewModels
{
    public class CourseStudentAssignmentViewModel
    {
        public string Name { get; set; }

        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int PassCredits { get; set; }

        public List<AssignmentStudentViewModel> Students { get; set; }
    }

    public class AssignmentStudentViewModel
    {
        public int StudentId { get; set; }

        public string StudentFullName { get; set; }

        public bool IsAssigned { get; set; }
    }
}
