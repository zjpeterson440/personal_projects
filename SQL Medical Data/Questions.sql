USE Healthcare_DB

--Finds the number of rows with a gross charger greater than $100
SELECT COUNT(*)
FROM FactTable
WHERE GrossCharge > 100

--Find # of unique patients in Healthcare_DB
SELECT
	COUNT(DISTINCT PatientNumber)
FROM dimPatient


--CPT codes in each cpt grouping

SELECT
	COUNT(DISTINCT CptCode) AS 'Number in Grouping' --There could be several CPT same CPT codes in each grouping. Distinct finds the correct #
	,CptGrouping
FROM dimCptCode 
GROUP BY CptGrouping
ORDER BY COUNT(DISTINCT CptCode) DESC

--How many providers submitted a medicare insurance claim?

SELECT
COUNT(Distinct dimPhysician.ProviderNpi) as 'CountofProviders'
FROM FactTable
INNER JOIN 
	dimPhysician ON dimPhysician.dimPhysicianPK = FactTable.dimPhysicianPK
INNER JOIN 
	dimPayer ON dimPayer.dimPayerPK = FactTable.dimPayerPK
WHERE dimPayer.dimPayerPK = '98735'

--How many providers submitted a claim under each category
SELECT
	dimPayer.PayerName
	,COUNT(Distinct dimPhysician.ProviderNpi) as 'CountofProviders'
FROM FactTable
INNER JOIN 
	dimPhysician ON dimPhysician.dimPhysicianPK = FactTable.dimPhysicianPK
INNER JOIN 
	dimPayer ON dimPayer.dimPayerPK = FactTable.dimPayerPK
GROUP BY PayerName

--Gross collection rate for each Location/Highest GCR location
SELECT
--TOP 1
dimLocation.LocationName
,FORMAT(-SUM(FactTable.Payment)/SUM(FactTable.GrossCharge),'P1') AS GCR
FROM FactTable
INNER JOIN dimLocation ON dimLocation.dimLocationPK = FactTable.dimLocationPK
GROUP BY dimLocation.LocationName
ORDER BY GCR DESC

--How many CPT Codes have more than 100 units

SELECT COUNT(*) AS 'CountofCPTGreaterThan100'
FROM(
SELECT
	CptCode
	,SUM(CPTUnits) AS Number_of_Units
FROM FactTable
INNER JOIN 
dimCptCode ON dimCPTCode.dimCPTCodePK = FactTable.dimCPTCodePK
GROUP BY dimCPTCode.CptCode
HAVING SUM(CPTUnits) >= 100) AS A

--Show the CPT Codes

SELECT
	CptCode
	,SUM(CPTUnits) AS Number_of_Units
FROM FactTable
INNER JOIN 
	dimCptCode ON dimCPTCode.dimCPTCodePK = FactTable.dimCPTCodePK
GROUP BY dimCPTCode.CptCode
HAVING SUM(CPTUnits) >= 100


--Which Physician specialty has received the highest ammount of payments

SELECT
TOP 1
	dimPhysician.ProviderSpecialty
	,-SUM(FactTable.Payment) AS highest_payment
FROM FactTable
INNER JOIN 
	dimPhysician ON dimPhysician.dimPhysicianPK = FactTable.dimPhysicianPK
GROUP BY dimPhysician.ProviderSpecialty
ORDER BY highest_payment DESC

--Payments by Month for Internal medicine
SELECT
	dimDate.MonthPeriod
	,dimDate.MonthYear
	,FORMAT(-SUM(FactTable.Payment), '$#,###') AS highest_payment
FROM FactTable
INNER JOIN 
	dimPhysician ON dimPhysician.dimPhysicianPK = FactTable.dimPhysicianPK
INNER JOIN
	dimDate on dimDate.dimDatePostPK = FactTable.dimDatePostPK
GROUP BY dimDate.MonthPeriod, ProviderSpecialty, dimDate.MonthYear
HAVING ProviderSpecialty = 'Internal Medicine'
ORDER BY MonthPeriod ASC

--How many CptUnits by DiagnosisCodeGroup are assigned as "J code" Diagnosis
SELECT
	DiagnosisCodeGroup
	,SUM(FactTable.CptUnits) AS 'Units'
FROM FactTable
INNER JOIN dimDiagnosisCode
	ON dimDiagnosisCode.dimDiagnosisCodePK = FactTable.dimDiagnosisCodePK
WHERE dimDiagnosisCode.DiagnosisCode LIKE 'J%'
GROUP BY DiagnosisCodeGroup

--Create age group report U18/18-65/over 65
--Columns must include First + Last name, email, age, city + state

SELECT
CONCAT(FirstName, ' ', LastName) AS 'Name'
,Email
,PatientAge AS 'Age'
,CONCAT(City, ', ', [State]) AS 'Location'
,CASE
	WHEN PatientAge < 18 THEN 'Under 18'
	WHEN PatientAge BETWEEN 18 AND 65 THEN '18-65'
	ELSE 'Over 65'
END AS 'Patient Age Field'
FROM dimPatient


--How many dollars have been written off due to credentials?

SELECT
	FORMAT(-SUM(FactTable.Adjustment),'$#,###')
FROM FactTable
INNER JOIN dimTransaction
	ON dimTransaction.dimTransactionPK = FactTable.dimTransactionPK
WHERE AdjustmentReason = 'Credentialing'


--Which Location has the most?
SELECT
	dimLocation.LocationName
	,-SUM(FactTable.Adjustment) AS 'credentialing_adj'
FROM FactTable
INNER JOIN dimTransaction
	ON dimTransaction.dimTransactionPK = FactTable.dimTransactionPK
INNER JOIN dimLocation
	ON dimLocation.dimLocationPK = FactTable.dimLocationPK
WHERE AdjustmentReason = 'Credentialing'
GROUP BY dimLocation.LocationName
ORDER BY credentialing_adj DESC

--How many physicians?

SELECT
	LocationName
	,COUNT(DISTINCT ProviderNPI) AS 'Physicians'
	,FORMAT(-SUM(FactTable.Adjustment),'$#,###') AS 'credentialing_adj'
FROM FactTable
INNER JOIN dimTransaction
	ON dimTransaction.dimTransactionPK = FactTable.dimTransactionPK
INNER JOIN dimLocation
	ON dimLocation.dimLocationPK = FactTable.dimLocationPK
INNER JOIN dimPhysician
	ON dimPhysician.dimPhysicianPK = FactTable.dimPhysicianPK
WHERE AdjustmentReason = 'Credentialing' AND dimLocation.LocationName = 'Angelstone Community Hospital'
GROUP BY LocationName

--List of physicians with the most Credentialing adjustments/Why?

SELECT
	dimPhysician.ProviderName
	,dimPhysician.ProviderNpi
	,dimTransaction.[Transaction]
	,-SUM(FactTable.Adjustment) AS 'credentialing_adj'
FROM FactTable
INNER JOIN dimTransaction
	ON dimTransaction.dimTransactionPK = FactTable.dimTransactionPK
INNER JOIN dimLocation
	ON dimLocation.dimLocationPK = FactTable.dimLocationPK
INNER JOIN dimPhysician
	ON dimPhysician.dimPhysicianPK = FactTable.dimPhysicianPK
WHERE AdjustmentReason = 'Credentialing' AND dimLocation.LocationName = 'Angelstone Community Hospital'
GROUP BY dimPhysician.ProviderNpi, dimPhysician.ProviderName, dimTransaction.[Transaction]
ORDER BY credentialing_adj DESC


--Construct a table with the following info: Location, # of Physicians, # of Patients, GrossCharge, Avg Charge/Patient
SELECT
	LocationName
	,COUNT(DISTINCT dimPhysician.ProviderNpi) AS '# Physicians'
	,COUNT(dimPatient.PatientNumber) AS '# of Patients'
	,FORMAT(SUM(FactTable.GrossCharge),'$#,###')
	,FORMAT(SUM(FactTable.GrossCharge)/COUNT(dimPatient.PatientNumber),'$#,###.##')
FROM FactTable
INNER JOIN dimPhysician
	ON dimPhysician.dimPhysicianPK = FactTable.dimPhysicianPK
INNER JOIN dimPatient
	ON dimPatient.dimPatientPK = FactTable.dimPatientPK
INNER JOIN dimLocation
	ON dimLocation.dimLocationPK = FactTable.dimLocationPK
GROUP BY LocationName
ORDER BY [# of Patients] DESC


--Averae patient age by gender for patients seen at Big Heart Community
--with a diagnosis that includes Type 2 diabetes

SELECT
	PatientGender
	,AVG(PatientAge) AS avg_patient_age
	,COUNT(PatientNumber) AS patient_count
FROM(
	SELECT DISTINCT
		dimPatient.PatientNumber
		,dimPatient.PatientAge
		,dimPatient.PatientGender
	FROM FactTable
	INNER JOIN dimPatient
		ON dimPatient.dimPatientPK = FactTable.dimPatientPK
	INNER JOIN dimLocation
		ON dimLocation.dimLocationPK = FactTable.dimLocationPK
	INNER JOIN dimDiagnosisCode
		ON dimDiagnosisCode.dimDiagnosisCodePK = FactTable.dimDiagnosisCodePK
	WHERE dimLocation.LocationName = 'Big Heart Community Hospital'
	AND dimDiagnosisCode.DiagnosisCodeDescription LIKE '%type 2%') AS A
GROUP BY PatientGender


/*
Compare: Office/outpatient visit est and Office/outpatient visit new
Need: Each CptCode, CptDesc and associated CptUnits
What is the charge per CptUnit?
*/

SELECT
	dimCptCode.CptDesc
	,dimCptCode.CptCode
	,SUM(FactTable.CPTUnits) AS 'cpt_unit_total'
	,FORMAT(SUM(FactTable.GrossCharge)/SUM(FactTable.CPTUnits),'$#.##') AS 'charge_per_cpt_unit'
FROM FactTable
INNER JOIN dimCptCode
	ON dimCptCode.dimCPTCodePK = FactTable.dimCPTCodePK
WHERE dimCptCode.CptDesc IN ('Office/outpatient visit est', 'Office/outpatient visit new')
GROUP BY dimCptCode.CptDesc, dimCptCode.CptCode
ORDER BY 1,2

/* Find payment per CPT Unit by PayerName
Where the visit type: Initial hospital care
Show CptCode, CptDesc, CptUnits
*/

SELECT
	dimCptCode.CptCode
	,dimCptCode.CptDesc
	,dimPayer.PayerName
	,SUM(FactTable.CptUnits) AS 'cpt_unit_total'
	,FORMAT(-SUM(FactTable.Payment)/NULLIF(SUM(FactTable.CPTUnits),0),'$#.##') AS 'payment_per_cpt'
FROM FactTable
INNER JOIN dimCptCode
	ON dimCptCode.dimCPTCodePK = FactTable.dimCPTCodePK
INNER JOIN dimPayer
	ON dimPayer.dimPayerPK = FactTable.dimPayerPK
WHERE dimCptCode.CptDesc = 'Initial hospital care'
GROUP BY 
	dimCptCode.CptCode
	,dimCptCode.CptDesc
	,dimPayer.PayerName


/* 1. Find the Net Charge (Gross - Contractual Adjustments)
	2. Find the Net Collection Rate for each physician specialty
		a. Which specialty has the worst Net Collection rate
			w/ a NetCharge greater than $25,000
*/

SELECT
	dimPhysician.ProviderSpecialty
	,SUM(FactTable.GrossCharge) AS 'gross_charges'
	,SUM(CASE WHEN AdjustmentReason = 'Contractual'
		THEN Adjustment
		ELSE NULL
		END) AS 'contract_adj'
	,SUM(FactTable.GrossCharge) + 
		SUM(CASE WHEN AdjustmentReason = 'Contractual'
			THEN Adjustment
			ELSE NULL
			END) AS 'net_charge'
	,FORMAT(-SUM(FactTable.Payment)/(SUM(FactTable.GrossCharge) + 
		SUM(CASE WHEN AdjustmentReason = 'Contractual'
			THEN Adjustment
			ELSE NULL
			END)),'P') AS 'gcr'
FROM FactTable
INNER JOIN dimTransaction
	ON dimTransaction.dimTransactionPK = FactTable.dimTransactionPK
INNER JOIN dimPhysician
	ON dimPhysician.dimPhysicianPK = FactTable.dimPhysicianPK
GROUP BY dimPhysician.ProviderSpecialty
HAVING (SUM(FactTable.GrossCharge) + 
		SUM(CASE WHEN AdjustmentReason = 'Contractual'
			THEN Adjustment
			ELSE NULL
			END)) > 25000
ORDER BY gcr ASC


--Same answer w/ sub query + add columns for data clarity

SELECT
	ProviderSpecialty
	,gross_charges
	,contract_adj
	,net_charge
	,Payments
	,AR
	,FORMAT(-Payments/net_charge,'P') AS 'GCR'
FROM(
	SELECT
		dimPhysician.ProviderSpecialty
		,SUM(FactTable.GrossCharge) AS 'gross_charges'
		,SUM(CASE WHEN AdjustmentReason = 'Contractual'
			THEN Adjustment
			ELSE NULL
			END) AS 'contract_adj'
		,SUM(FactTable.GrossCharge) + 
			SUM(CASE WHEN AdjustmentReason = 'Contractual'
				THEN Adjustment
				ELSE NULL
				END) AS 'net_charge'
		,SUM(FactTable.Payment) AS 'Payments'
		,SUM(FactTable.AR) AS 'AR'
	FROM FactTable
	INNER JOIN dimTransaction
		ON dimTransaction.dimTransactionPK = FactTable.dimTransactionPK
	INNER JOIN dimPhysician
		ON dimPhysician.dimPhysicianPK = FactTable.dimPhysicianPK
	GROUP BY dimPhysician.ProviderSpecialty) AS A
WHERE net_charge > 25000
ORDER BY GCR ASC


/*
More questions to answer:

Number of specialties in each hospital
How much do they make?
	-Concat?
	-In rows vs columns?
	-Which group has the highest GCR

Number of return patient visits per department?

Do costs go up if patients return? How about Gross proffit vs Revenue?
	-CPT units?


	*/