-- 1️⃣ Question:
-- In a transaction, if I perform multiple updates and an error happens in the third statement, but I have not used SAVEPOINT, what will happen if I issue a ROLLBACK?
-- Will my first two updates persist?
Answer : 
-> The entire transaction will be failed and rollback will undo all changes.
-> No partial changes

begin transaction;
    update table1 set col1 = new_value;
    update table1 set col2 = new_value;
    update table1 set non_exist_col = new_value; -- error statement
rollback;

-> No changes will happen at table1 

-- 2️⃣ Question:note
-- Suppose Transaction A updates Alice’s balance but does not commit. Can Transaction B read the new balance if the isolation level is set to READ COMMITTED?
Answer :
-> Even it's READ COMMITTED islation level, It won't show the uncommitted changes made by other transaction.

begin transaction; -- Transaction A
update tbl_bank_accounts set balance = 500 where name='Alice';

begin; -- Transaction B
select balance from tbl_bank_accounts where name='Alice';
commit;

-> Transaction B only show the old balance

-- 3️⃣ Question:
-- What will happen if two concurrent transactions both execute:
-- UPDATE tbl_bank_accounts SET balance = balance - 100 WHERE account_name = 'Alice';
-- at the same time? Will one overwrite the other?
Answer :
-> Both of the changes will be applied to the row.
-> But it will not rewrite the each other changes. Instead it will rewrite change with the latest change by row-level locking.


-- 4️⃣ Question:
-- If I issue ROLLBACK TO SAVEPOINT after_alice;, will it only undo changes made after the savepoint or everything?
Answer :
-> It will undo the changes made after the savepoint.

begin transaction;
    update tbl_bank_accounts set balance = 500 where name='Alice'; -- remains changed
    rollback to savepoint after_alice;                             -- savepoint
    update tbl_bank_accounts set balance = 500 where name='Alice'; -- will not remain the changes
commit;

-- 5️⃣ Question:
-- Which isolation level in PostgreSQL prevents phantom reads?
Answer :
-> The Serializable isolation level will prevent phantom reads.
-> Force a retry

-- 6️⃣ Question:
-- Can Postgres perform a dirty read (reading uncommitted data from another transaction)?
Answer :
-> Postgres does not allow dirty reads.
-> If other transaction has not committed yet, it will blocks the read or shows last committed changes.

-- 7️⃣ Question:
-- If autocommit is ON (default in Postgres), and I execute an UPDATE, is it safe to assume the change is immediately committed?
Answer :
-> Yes, The update statement will be committed immediately if autocommit is on.
-> It did not commit automatically only when it wrapped up with transaction.

-- 8️⃣ Question:
-- If I do this:

BEGIN;
UPDATE accounts SET balance = balance - 500 WHERE id = 1;
-- (No COMMIT yet)
-- And from another session, I run:

SELECT balance FROM accounts WHERE id = 1;

Answer :
-> It will read the last committed version of the row.
-> Postgres uses MVCC.
    -> Each transaction has the snapshot of database before the transaction start.
    -> Transaction B will not see the changes of Transaction A untill it is committed.