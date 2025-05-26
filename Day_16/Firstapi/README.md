Requests :

Patient 

    Get     -   http://localhost:5035/api/patient           - Returns Collection of Patients 
                http://localhost:5035/api/patient/{id}      - Returns Patients object
    Post    -   http://localhost:5035/api/patient

        body => {
                    "id": 101,
                    "name": "Nithish",
                    "age": 20
                }
    Put     -   http://localhost:5035/api/patient/{id}

        body => {
                    "id": 101,
                    "name": "Nithish Vikky",
                    "age": 22
                }
    Delete  -   http://localhost:5035/api/patient/{id}

