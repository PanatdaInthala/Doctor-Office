using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace DoctorOffice.Models
{
  public class Patient
  {
    private string _name;
    private int _id;
    private string _birth_date;

    public Patient(string name, string birth_date, int id = 0)
    {
      _name = name;
      _id = id;
      _birth_date = birth_date;
    }
    
    public override bool Equals(System.Object otherPatient)
    {
      if (!(otherPatient is Patient))
      {
        return false;
      }
      else
      {
        Patient newPatient = (Patient) otherPatient;
        return this.GetId().Equals(newPatient.GetId());
      }
    }

    public override int GetHashCode()
    {
      return this.GetId().GetHashCode();
    }

    public string GetName()
    {
      return _name;
    }

    public int GetId()
    {
      return _id;
    }

    public string GetBirthDate()
    {
      return _birth_date;
    }

    public static List<Patient> GetAll()
    {
      List<Patient> allPatients = new List<Patient>{};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM patients;";
      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int patientId = rdr.GetInt32(0);
        string patientName = rdr.GetString(1);
        string patientBirthDate = rdr.GetString(2);
        Patient newPatient = new Patient(patientName, patientBirthDate, patientId);
        allPatients.Add(newPatient);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return allPatients;
    }

    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO patients (name, birth_date) VALUES (@name, @birth_date);";

      MySqlParameter name = new MySqlParameter();
      name.ParameterName = "@name";
      name.Value = this._name;
      cmd.Parameters.Add(name);

      MySqlParameter birth_date = new MySqlParameter();
      birth_date.ParameterName = "@birth_date";
      birth_date.Value = this._birth_date;
      cmd.Parameters.Add(birth_date);

      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId;
      conn.Close();

      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public static void DeleteAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM patients;";
      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public List<Doctor> GetDoctors()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT doctor_id FROM doctors_patients WHERE patient_id = @patientId;";

      MySqlParameter patientIdParameter = new MySqlParameter();
      patientIdParameter.ParameterName = "@patientId";
      patientIdParameter.Value = _id;
      cmd.Parameters.Add(patientIdParameter);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;

      List<int> doctorIds = new List<int> {};
      while(rdr.Read())
      {
        int doctorId = rdr.GetInt32(0);
        doctorIds.Add(doctorId);
      }
      rdr.Dispose();

      List<Doctor> doctors = new List<Doctor> {};
      foreach (int doctorId in doctorIds)
      {
        var doctorQuery = conn.CreateCommand() as MySqlCommand;
        doctorQuery.CommandText = @"SELECT * FROM doctors WHERE id = @DoctorId;";

        MySqlParameter doctorIdParameter = new MySqlParameter();
        doctorIdParameter.ParameterName = "@DoctorId";
        doctorIdParameter.Value = doctorId;
        doctorQuery.Parameters.Add(doctorIdParameter);

        var doctorQueryRdr = doctorQuery.ExecuteReader() as MySqlDataReader;
        while(doctorQueryRdr.Read())
        {
          int thisDoctorId = doctorQueryRdr.GetInt32(0);
          string doctorName = doctorQueryRdr.GetString(1);

          Doctor foundDoctor = new Doctor(doctorName, thisDoctorId);
          doctors.Add(foundDoctor);
        }
        doctorQueryRdr.Dispose();
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return doctors;
    }

    public void AddDoctor(Doctor newDoctor)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO doctors_patients (doctor_id, patient_id) VALUES (@DoctorId, @PatientId);";

      MySqlParameter doctor_id = new MySqlParameter();
      doctor_id.ParameterName = "@DoctorId";
      doctor_id.Value = newDoctor.GetId();
      cmd.Parameters.Add(doctor_id);

      MySqlParameter patient_id = new MySqlParameter();
      patient_id.ParameterName = "@PatientId";
      patient_id.Value = _id;
      cmd.Parameters.Add(patient_id);

      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public void Delete()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM patients WHERE id = @patientId; DELETE FROM doctors_patients WHERE patient_id = @patientId;";

      MySqlParameter patientIdParameter = new MySqlParameter();
      patientIdParameter.ParameterName = "@patientId";
      patientIdParameter.Value = this.GetId();
      cmd.Parameters.Add(patientIdParameter);

      cmd.ExecuteNonQuery();
      if (conn != null)
      {
        conn.Close();
      }
    }

    public static Patient Find(int id)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM patients WHERE id = (@searchId);";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = id;
      cmd.Parameters.Add(searchId);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      int patientId = 0;
      string patientName = "";
      string patientBirthDate = "";

      while(rdr.Read())
      {
        patientId = rdr.GetInt32(0);
        patientName = rdr.GetString(1);
        patientBirthDate = rdr.GetString(2);
      }

      Patient newPatient = new Patient(patientName, patientBirthDate, patientId);
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }

      return newPatient;
    }

    public void Edit(string newName, string newBirthDate)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"UPDATE patients SET name = @newName, birth_date =  @newBirthDate WHERE id = @searchId;";

      // this is not correct, but why?

      // @"UPDATE patients SET (name, birth_date) = (@newName, @newBirthDate) WHERE id = @searchId;";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = _id;
      cmd.Parameters.Add(searchId);

      MySqlParameter name = new MySqlParameter();
      name.ParameterName = "@newName";
      name.Value = newName;
      cmd.Parameters.Add(name);

      MySqlParameter birthDate = new MySqlParameter();
      birthDate.ParameterName = "@newBirthDate";
      birthDate.Value = newBirthDate;
      cmd.Parameters.Add(birthDate);

      cmd.ExecuteNonQuery();
      _name = newName;
      _birth_date = newBirthDate;

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }
  }
}
