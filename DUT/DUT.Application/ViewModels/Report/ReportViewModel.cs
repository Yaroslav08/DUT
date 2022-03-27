﻿using DUT.Application.ViewModels.Subject;
using DUT.Domain.Models;

namespace DUT.Application.ViewModels.Report
{
    public class ReportViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Title { get; set; }
        public bool IsDraft { get; set; }
        public ReportType Type { get; set; }
        public List<Student> CalculatedMarks { get; set; }
        public List<Student> Marks { get; set; }
        public SubjectViewModel Subject { get; set; }
    }
}