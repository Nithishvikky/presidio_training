-- Phase 6: Security & Roles

-- 1. Create a `readonly_user` role:
--    * Can run `SELECT` on `students`, `courses`, and `certificates`
--    * Cannot `INSERT`, `UPDATE`, or `DELETE`
   
-- 2. Create a `data_entry_user` role:
--    * Can `INSERT` into `students`, `enrollments`
--    * Cannot modify certificates directly

create role readonly_user with login password 'readonly123';
grant usage on schema public to readonly_user;
grant select on students,courses,certificates to readonly_user;


create role data_entry_user with login password 'entry123';
grant usage on schema public to data_entry_user;
grant insert on students,enrollments to data_entry_user;

-- For serial columns in the table, It should use the select query to get the next serial value
-- So we should give grant permission for the sequences
-- ERROR:  permission denied for sequence students_student_id_seq 
-- ERROR:  permission denied for sequence enrollments_enrollment_id_seq 
grant usage, select on sequence students_student_id_seq to data_entry_user;
grant usage, select on sequence enrollments_enrollment_id_seq to data_entry_user;