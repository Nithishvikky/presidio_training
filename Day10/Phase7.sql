-- Phase 7: Transactions & Atomicity
-- Write a transaction block that:
-- * Enrolls a student
-- * Issues a certificate
-- * Fails if certificate generation fails (rollback)

-- ```sql
-- BEGIN;
-- -- insert into enrollments
-- -- insert into certificates
-- -- COMMIT or ROLLBACK on error
-- ```

begin;

with inserted_enrollment as(
	insert into enrollments(student_id,course_id)
	values(5,1)
	returning enrollment_id
)
insert into certificates(enrollment_id,serial_no)
select enrollment_id, 'CERT'||To_char(now(),'YY')||lpad(enrollment_id::text,4,'0') -- exists id
from inserted_enrollment;

commit;
rollback;

select * from enrollments;
select * from certificates;