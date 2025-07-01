sequenceDiagram
    participant U   as User
    participant NG  as Angular Frontend
    participant IMG as Image API (Cloud Run)
    participant DB  as Cloud SQL (SQL Server)
    participant GCS as Cloud Storage
	%% --- async thumbnail pathway ---
    participant EVT as Eventarc
    participant RZ  as Resize API (Cloud Run)

    U->>NG: Select file (≤20 MB)
    NG->>IMG: POST /api/image/upload<br/>(multipart form-data) + JWT
    IMG->>IMG: Validate JWT → uid
    IMG->>DB: INSERT INTO FILES (..., STATUS = processing)<br/>RETURNING ID
    DB-->>IMG: new ID
    IMG->>GCS: upload object “ID.ext”
    GCS-->>IMG: 200 Upload OK
    IMG-->>NG: 200 OK {"success":true}
    NG-->>U: Show “Upload success” + refresh list

    %% --- async thumbnail pathway ---
    GCS-->>EVT: Object Finalize event
    EVT-->>RZ: HTTP trigger (/resize) with object info
    RZ->>GCS: download original, generate thumbnail & upload thumb
    RZ->>DB: UPDATE FILES SET STATUS='done', THUMB_PATH='gs://...'
    RZ-->>EVT: 200 OK