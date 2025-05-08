using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Pronto.ValuationApi.Data.Models
{
    public class Valuation
    {
        public string? Id { get; set; }
        public string AdjusterUserId { get; set; }
        public string Status { get; set; }
        public DateTime AccidentDate { get; set; }
        public string? AccidentLocation { get; set; }
        public string? PolicyNumber { get; set; }

        public Stakeholder? Stakeholder { get; set; }
        public Applicant? Applicant { get; set; }
        public VehicleDetails? VehicleDetails { get; set; }
        public List<DocumentUpload> Documents { get; set; } = new();
        public List<ComponentInspection> Components { get; set; } = new();
        public ValuationSummary? Summary { get; set; }
        public List<WorkflowStep> Workflow { get; set; } = new();

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class Stakeholder
    {
        public string? Name { get; set; }
        public string? ExecutiveName { get; set; }
        public string? ExecutiveContact { get; set; }
        public string? ExecutiveWhatsapp { get; set; }
        public Applicant? Applicant { get; set; }
    }

    public class Applicant
    {
        public string? Name { get; set; }
        public string? Contact { get; set; }
        public VehicleDetails? VehicleDetails { get; set; }
    }

    public class VehicleDetails
    {
        public string? RegistrationNumber { get; set; }
        public string? Segment { get; set; }
        public string? Make { get; set; }
        public string? Model { get; set; }
        public int? YearMfg { get; set; }
        public string? PresentAddress { get; set; }
        public string? PermanentAddress { get; set; }
        public bool? Hypothecation { get; set; }
        public string? Insurer { get; set; }
        public DateTime? DateOfRegistration { get; set; }
        public string? VehicleClass { get; set; }
        public int? EngineCC { get; set; }
        public decimal? Gvw { get; set; }
        public int? SeatingCapacity { get; set; }
        public DateTime? PolicyValidUpTo { get; set; }
        public decimal? Idv { get; set; }
        public string? PermitNo { get; set; }
        public DateTime? PermitValidUpTo { get; set; }
        public List<DocumentUpload> Documents { get; set; } = new();
    }

    public class DocumentUpload
    {
        public string? Type { get; set; }
        public string? FilePath { get; set; }
        public DateTime? UploadedAt { get; set; }
        public List<ComponentInspection> Components { get; set; } = new();
    }

    public class ComponentInspection
    {
        public int? ComponentTypeId { get; set; }
        public string? Condition { get; set; }
        public string? Remarks { get; set; }
        public ValuationSummary? Summary { get; set; }
    }

    public class ValuationSummary
    {
        public double? OverallRating { get; set; }
        public decimal? ValuationAmount { get; set; }
        public string? ChassisPunch { get; set; }
        public string? Remarks { get; set; }
        public List<WorkflowStep> Workflow { get; set; } = new();       
    }

    public class WorkflowStep
    {
        public int? TemplateStepId { get; set; }
        public int? StepOrder { get; set; }
        public string? AssignedToRole { get; set; }
        public string? Status { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

    public class ComponentType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class WorkflowStepTemplate
    {
        public int Id { get; set; }
        public int StepOrder { get; set; }
        public string StepName { get; set; }
        public string Description { get; set; }
    }
}