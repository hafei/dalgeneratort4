

using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using DG.Common;

namespace DataGenerator
{
public class StateDao

{
		 
			private const string IdentityColumn = "Id"; 
				private const string CountryIdColumn ="CountryId";
				private const string NameColumn ="Name";
				private const string AbbrevColumn ="Abbrev";
				private static readonly string[] ColumnCollections = new[] { IdentityColumn,CountryIdColumn,NameColumn,AbbrevColumn};
        private readonly string _columnCollectionSelectString = string.Join(",", ColumnCollections);
        private static readonly string[] KeyColumns = new string[] { "Id" };
        private static readonly string WhereClause = KeyColumns.JoinFormat(" AND ", "{0} = @{0}");
		private readonly Database _database;
      
        public StateDao()
        {
            _database = DatabaseFactory.CreateDatabase("QStudyDB");
        }

        public StateDao(Database database)
        {
            _database = database;
        }
		
		private string GetSortText(SortDefinitions sortDefinitions)
		{
			string sortDefinition;
            if(sortDefinitions!=null && sortDefinitions.SoftItems.Count>0)
            {
                sortDefinition = string.Join(" , ", sortDefinitions.SoftItems.Select(p=> string.Format("{0} {1}", p.ColumnName , (p.SortOrder==SortOrder.None)?"":p.SortOrder.ToString())));
				
				sortDefinition = string.Format(" ORDER BY {0}", sortDefinition);				
            }
			else
			{
				 sortDefinition = "ORDER BY " + string.Join(",", KeyColumns);
			}		
			return sortDefinition;
		}
		
		public List<State> GetStates( SortDefinitions sortDefinitions , int startIndex, int pageSize)
        {
		    string sqlText;
			var sortText = GetSortText(sortDefinitions);
			if(startIndex>-1 && pageSize>0)
			{
			  sqlText = GetPagedQuery(sortText, startIndex, pageSize);
			}
			else
			 sqlText = GetStateSQL();

            return _database.CreateSqlStringAccessor<State>(sqlText).Execute().ToList();
        }
		
		
		

        public List<State> GetStates()
        {

            return _database.CreateSqlStringAccessor<State>(GetStateSQL()).Execute().ToList();
        }

                public State GetStateById(object id)
        {
            return _database.CreateSqlStringAccessor<State>(GetStateByIdSQL(), new StateSelectIdParameterMapper()).Execute(id).SingleOrDefault();
        }
		
        private class StateSelectIdParameterMapper : IParameterMapper
        {
		   public void AssignParameters(DbCommand command, object[] parameterValues)
            {
				var parameter = command.CreateParameter();
                parameter.ParameterName = string.Format("@{0}", IdentityColumn);
                parameter.Value = parameterValues[0];
                command.Parameters.Add(parameter);
			}
        }
		
		private string GetStateByIdSQL()
        {
            var sql = string.Format(@"SELECT  {0}
                            FROM 
                                States
                            WHERE
                                {1}
                                       ", _columnCollectionSelectString, string.Format("{0} = @{0}",IdentityColumn));
            return sql;
        }
		        public long Insert(State state)
        { 			
			var command = _database.GetSqlStringCommand(InsertStateSQL());
		   			var parameter = command.CreateParameter();
            parameter.ParameterName = string.Format("@{0}", IdentityColumn);
            parameter.DbType = DbType.Int64;
            parameter.Direction = ParameterDirection.Output;
            command.Parameters.Add(parameter);
			            AddInsertParameters(command, state);
            _database.ExecuteNonQuery(command);
			            var id =Convert.ToInt64( parameter.Value);
            return id;
			        }

        public void Update(State state)
        {
            var command = _database.GetSqlStringCommand(UpdateStateSQL());
            AddUpdateParameters(command, state);
            _database.ExecuteNonQuery(command);
        }

        public void Delete(State state)
        {
            var command = _database.GetSqlStringCommand(DeleteStateSQL());
			            AddIdentityColumn(command, state);
			            _database.ExecuteNonQuery(command);
        }
				
		private void AddInsertParameters(DbCommand command, State state)
        {
            DbParameter parameter = null;
        			    parameter = command.CreateParameter();
            parameter.ParameterName = string.Format("@{0}",@CountryIdColumn);
			            parameter.Value = state.CountryId;
            			command.Parameters.Add(parameter);
		
					    parameter = command.CreateParameter();
            parameter.ParameterName = string.Format("@{0}",@NameColumn);
			            parameter.Value = state.Name;
            			command.Parameters.Add(parameter);
		
					    parameter = command.CreateParameter();
            parameter.ParameterName = string.Format("@{0}",@AbbrevColumn);
						parameter.Value = (object)state.Abbrev??DBNull.Value;
						command.Parameters.Add(parameter);
		
			           
        }		
		
		private void AddUpdateParameters(DbCommand command, State state)
        {
		            AddIdentityColumn(command, state);
		            AddInsertParameters(command, state);
        }
				
        private void AddIdentityColumn(DbCommand command, State state)
        {
		  
            var parameter = command.CreateParameter();
            parameter.ParameterName = string.Format("@{0}", IdentityColumn);
            parameter.Value = state.Id;
            command.Parameters.Add(parameter);

		}
				
		private string GetStateSQL()
        {
            var sql = string.Format(@" SELECT  {0} 
                                          FROM 
                                                States
                                       ", _columnCollectionSelectString);
            return sql;
        }      

		private string GetPagedQuery( string sqlText, int startIndex , int pageSize)
		{
		  	var endIndex = startIndex + pageSize;
          	startIndex = startIndex + 1;

          	var sql = string.Format(@"SELECT TOP {0} {4} 
						FROM ( 
							SELECT    {4} , ROW_NUMBER() OVER ({1}) AS ROW 
						    FROM  States ) AS WithRowNumber
					    WHERE
                            ROW BETWEEN {2} AND {3} {1}", pageSize, sqlText, startIndex, endIndex, _columnCollectionSelectString);

          	return sql;		    
		}
		
        private string InsertStateSQL()
        {
			var nonIdentColumns= ColumnCollections.Except(new[]{IdentityColumn}).ToList();
		 	var sql = string.Format(@" INSERT INTO
                            States (
                              {0}
                            )
                            VALUES
                            (
                                {1}
                            ) ;
                                       ", nonIdentColumns.JoinFormat(",","{0}"), nonIdentColumns.JoinFormat(",", "@{0}"));
                        sql = string.Format("{0} SET @{1} = SCOPE_IDENTITY()", sql, IdentityColumn);
                        return sql;
        }

        private string UpdateStateSQL()
        {
		 	var sql = string.Format(@" UPDATE
                                 States
                            SET
                               {1}
                            WHERE 
                                  {0}
                                       ", WhereClause, ColumnCollections.Except(KeyColumns).ToArray().JoinFormat(" , ", "{0} = @{0}"));
            return sql;
        }

        private string DeleteStateSQL()
        { 
		 	var sql = string.Format(@" DELETE  FROM
                              States
                            WHERE 
                                {0}           
                                       ", string.Format("{0}=@{0}",IdentityColumn));
            return sql;
        }
  
	}
public class CountryDao

{
		 
			private const string IdentityColumn = "Id"; 
				private const string ISOColumn ="ISO";
				private const string NameColumn ="Name";
				private const string PrintableNameColumn ="PrintableName";
				private const string ISO3Column ="ISO3";
				private const string NumCodeColumn ="NumCode";
				private const string HasStateColumn ="HasState";
				private static readonly string[] ColumnCollections = new[] { IdentityColumn,ISOColumn,NameColumn,PrintableNameColumn,ISO3Column,NumCodeColumn,HasStateColumn};
        private readonly string _columnCollectionSelectString = string.Join(",", ColumnCollections);
        private static readonly string[] KeyColumns = new string[] { "Id","ISO" };
        private static readonly string WhereClause = KeyColumns.JoinFormat(" AND ", "{0} = @{0}");
		private readonly Database _database;
      
        public CountryDao()
        {
            _database = DatabaseFactory.CreateDatabase("QStudyDB");
        }

        public CountryDao(Database database)
        {
            _database = database;
        }
		
		private string GetSortText(SortDefinitions sortDefinitions)
		{
			string sortDefinition;
            if(sortDefinitions!=null && sortDefinitions.SoftItems.Count>0)
            {
                sortDefinition = string.Join(" , ", sortDefinitions.SoftItems.Select(p=> string.Format("{0} {1}", p.ColumnName , (p.SortOrder==SortOrder.None)?"":p.SortOrder.ToString())));
				
				sortDefinition = string.Format(" ORDER BY {0}", sortDefinition);				
            }
			else
			{
				 sortDefinition = "ORDER BY " + string.Join(",", KeyColumns);
			}		
			return sortDefinition;
		}
		
		public List<Country> GetCountries( SortDefinitions sortDefinitions , int startIndex, int pageSize)
        {
		    string sqlText;
			var sortText = GetSortText(sortDefinitions);
			if(startIndex>-1 && pageSize>0)
			{
			  sqlText = GetPagedQuery(sortText, startIndex, pageSize);
			}
			else
			 sqlText = GetCountrySQL();

            return _database.CreateSqlStringAccessor<Country>(sqlText).Execute().ToList();
        }
		
		
		

        public List<Country> GetCountries()
        {

            return _database.CreateSqlStringAccessor<Country>(GetCountrySQL()).Execute().ToList();
        }

                public Country GetCountryById(object id)
        {
            return _database.CreateSqlStringAccessor<Country>(GetCountryByIdSQL(), new CountrySelectIdParameterMapper()).Execute(id).SingleOrDefault();
        }
		
        private class CountrySelectIdParameterMapper : IParameterMapper
        {
		   public void AssignParameters(DbCommand command, object[] parameterValues)
            {
				var parameter = command.CreateParameter();
                parameter.ParameterName = string.Format("@{0}", IdentityColumn);
                parameter.Value = parameterValues[0];
                command.Parameters.Add(parameter);
			}
        }
		
		private string GetCountryByIdSQL()
        {
            var sql = string.Format(@"SELECT  {0}
                            FROM 
                                Countries
                            WHERE
                                {1}
                                       ", _columnCollectionSelectString, string.Format("{0} = @{0}",IdentityColumn));
            return sql;
        }
		        public long Insert(Country country)
        { 			
			var command = _database.GetSqlStringCommand(InsertCountrySQL());
		   			var parameter = command.CreateParameter();
            parameter.ParameterName = string.Format("@{0}", IdentityColumn);
            parameter.DbType = DbType.Int64;
            parameter.Direction = ParameterDirection.Output;
            command.Parameters.Add(parameter);
			            AddInsertParameters(command, country);
            _database.ExecuteNonQuery(command);
			            var id =Convert.ToInt64( parameter.Value);
            return id;
			        }

        public void Update(Country country)
        {
            var command = _database.GetSqlStringCommand(UpdateCountrySQL());
            AddUpdateParameters(command, country);
            _database.ExecuteNonQuery(command);
        }

        public void Delete(Country country)
        {
            var command = _database.GetSqlStringCommand(DeleteCountrySQL());
			            AddIdentityColumn(command, country);
			            _database.ExecuteNonQuery(command);
        }
				
		private void AddInsertParameters(DbCommand command, Country country)
        {
            DbParameter parameter = null;
        			    parameter = command.CreateParameter();
            parameter.ParameterName = string.Format("@{0}",@ISOColumn);
			            parameter.Value = country.ISO;
            			command.Parameters.Add(parameter);
		
					    parameter = command.CreateParameter();
            parameter.ParameterName = string.Format("@{0}",@NameColumn);
			            parameter.Value = country.Name;
            			command.Parameters.Add(parameter);
		
					    parameter = command.CreateParameter();
            parameter.ParameterName = string.Format("@{0}",@PrintableNameColumn);
			            parameter.Value = country.PrintableName;
            			command.Parameters.Add(parameter);
		
					    parameter = command.CreateParameter();
            parameter.ParameterName = string.Format("@{0}",@ISO3Column);
						parameter.Value = (object)country.ISO3??DBNull.Value;
						command.Parameters.Add(parameter);
		
					    parameter = command.CreateParameter();
            parameter.ParameterName = string.Format("@{0}",@NumCodeColumn);
						parameter.Value = (object)country.NumCode??DBNull.Value;
						command.Parameters.Add(parameter);
		
					    parameter = command.CreateParameter();
            parameter.ParameterName = string.Format("@{0}",@HasStateColumn);
			            parameter.Value = country.HasState;
            			command.Parameters.Add(parameter);
		
			           
        }		
		
		private void AddUpdateParameters(DbCommand command, Country country)
        {
		            AddIdentityColumn(command, country);
		            AddInsertParameters(command, country);
        }
				
        private void AddIdentityColumn(DbCommand command, Country country)
        {
		  
            var parameter = command.CreateParameter();
            parameter.ParameterName = string.Format("@{0}", IdentityColumn);
            parameter.Value = country.Id;
            command.Parameters.Add(parameter);

		}
				
		private string GetCountrySQL()
        {
            var sql = string.Format(@" SELECT  {0} 
                                          FROM 
                                                Countries
                                       ", _columnCollectionSelectString);
            return sql;
        }      

		private string GetPagedQuery( string sqlText, int startIndex , int pageSize)
		{
		  	var endIndex = startIndex + pageSize;
          	startIndex = startIndex + 1;

          	var sql = string.Format(@"SELECT TOP {0} {4} 
						FROM ( 
							SELECT    {4} , ROW_NUMBER() OVER ({1}) AS ROW 
						    FROM  Countries ) AS WithRowNumber
					    WHERE
                            ROW BETWEEN {2} AND {3} {1}", pageSize, sqlText, startIndex, endIndex, _columnCollectionSelectString);

          	return sql;		    
		}
		
        private string InsertCountrySQL()
        {
			var nonIdentColumns= ColumnCollections.Except(new[]{IdentityColumn}).ToList();
		 	var sql = string.Format(@" INSERT INTO
                            Countries (
                              {0}
                            )
                            VALUES
                            (
                                {1}
                            ) ;
                                       ", nonIdentColumns.JoinFormat(",","{0}"), nonIdentColumns.JoinFormat(",", "@{0}"));
                        sql = string.Format("{0} SET @{1} = SCOPE_IDENTITY()", sql, IdentityColumn);
                        return sql;
        }

        private string UpdateCountrySQL()
        {
		 	var sql = string.Format(@" UPDATE
                                 Countries
                            SET
                               {1}
                            WHERE 
                                  {0}
                                       ", WhereClause, ColumnCollections.Except(KeyColumns).ToArray().JoinFormat(" , ", "{0} = @{0}"));
            return sql;
        }

        private string DeleteCountrySQL()
        { 
		 	var sql = string.Format(@" DELETE  FROM
                              Countries
                            WHERE 
                                {0}           
                                       ", string.Format("{0}=@{0}",IdentityColumn));
            return sql;
        }
  
	}
}
