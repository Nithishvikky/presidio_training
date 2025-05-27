
using ConsultingManagement.Interfaces;
using ConsultingManagement.Models;
using ConsultingManagement.Repositories;
using ConsultingManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConsultingManagement.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        IRepository<int, Appointment> _appointmentRepo = new AppointmentRepository();
        // IAppointmentService _appointmentService = new AppointmentService(_appointmentRepo);
    }
}