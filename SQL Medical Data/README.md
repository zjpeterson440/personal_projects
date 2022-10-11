The `healthcare_data_worksheet.xlsx` is a fictional Healthcare database designed to practice SQL queries on healthcare datasets.
It uses a star schema and has thousands of records. The SQL code in `Questions.sql` was written in `SQL Server Management Studio` and will continually added to and updated.


Variables in Database:

* `FactTablePK`	Unique identifier (Primary Key) for the fact table. 
* `dimPatientPK`	Unique identifier that joins the patient table to the fact table
* `dimPhysicianPK`	Unique identifier that joins the physician table to the fact table
* `dimDatePostPK`	Text formatted date that specifies the day the transaction was posted to the billing system
* `dimDateServicePK`	Text formatted date that specifies the day the patient was seen by the physician
* `dimCPTCodePK`	Unique identifier that joins the CPT table to the fact table
* `dimPayerPK`	Unique identifier that joins the payer table to the fact table
* `dimTransactionPK`	Unique identifier that joins the transaction table to the fact table
* `dimLocationPK`	Unique identifier that joins the location table to the fact table
* `PatientNumber`	Unique Patient identifier. This is often times called a Medical Record Number (MRN)
* `dimDiagnosisCodePK`	Unique identifier that joins the patient table to the fact table
* `CPTUnits`	The number of times the CPT Code has been performed. 
* `GrossCharge`	The gross charge for the CPT Code. This includes the contractual adjustment
* `Payment`	Payment received from payer and patient
* `Adjustment`	Any dollars from the gross charge that will not be collected
* `AR`	Outstanding accounts receivable that have yet to be collected or written off. 
* `FirstName`	First Name of the patient
* `LastName`	Last Name of the patient
* `Email`	Patients email address
* `PatientGender`	Gender/Sex of the patient
* `PatientAge`	Patients age
* `City`	City that the patient lives in
* `State`	State that the patient lives in
* `TransactionType`	Clarifies whether the transaction was a charge, payment, adjustment, etc. 
* `Transaction`	Specific transaction descriptions
* `AdjustmentReason`	Groups adjustments into operational reason groups
* `ProviderNpi`	Unique number assigned to each physician upon completion of medical school
* `ProviderName`	Physicians last name
* `ProviderSpecialty`	Area which the physician has specialized
* `ProviderFTE`	Physician time spent working on a weekly basis
* `PayerName`	Payer category
* `LocationName`	Field that specifics where the service was rendered
* `DiagnosisCode`	Code assigned by the physician to specific the patients diagnosis
* `DiagnosisCodeDescription`	Description to clarify the diagnosis code
* `DiagnosisCodeGroup`	Group of Diagnosis Codes
* `CptCode`	Five digit Current Procedural Terminology used to assign services rendered to the patient
* `CptDesc`	Specific CPT description
* `CptGrouping`	Groups the CPT codes into types of services
* `Date`	Date format - either date service was rendered or date of a transaction
* `Year`	Year - either date service was rendered or date of a transaction
* `Month	Month` - either date service was rendered or date of a transaction
* `MonthPeriod`	YYYYMM Text Format - either date service was rendered or date of a transaction
* `MonthYear`	Month and Year - either date service was rendered or date of a transaction
* `Day`	Day of Month Number - either date service was rendered or date of a transaction
* `DayName`	Day of week name - either date service was rendered or date of a transaction

