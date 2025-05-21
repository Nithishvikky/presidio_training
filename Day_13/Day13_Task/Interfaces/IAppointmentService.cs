using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Day13_Task.Models;

namespace Day13_Task.Interfaces
{
    public interface IAppointmentService
    {
        int AddAppointment(Appointment appointment);
        List<Appointment>? SearchAppointment(AppointmentSearchModel searchModel);

        List<Appointment>? AllAppointments();
    }
}
