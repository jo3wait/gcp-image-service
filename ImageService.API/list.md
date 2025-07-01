sequenceDiagram
    participant U   as User (Browser)
    participant NG  as Angular Frontend
    participant IMG as Image API (Cloud Run)
    participant DB  as Cloud SQL (SQL Server)

    U->>NG: Click ¡§Image List¡¨
    NG->>IMG: GET /api/image/list<br/>Authorization: Bearer JWT
    IMG->>IMG: Validate JWT ¡÷ extract User ID
    IMG->>DB: SELECT * FROM FILES WHERE USER_ID = uid
    DB-->>IMG: rows
    IMG-->>NG: 200 OK ¡qFileDto[]¡r
    NG-->>U: Render table
