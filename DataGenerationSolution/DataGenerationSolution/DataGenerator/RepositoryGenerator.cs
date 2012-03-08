

using System.Collections.Generic;
using DG.Common;

namespace DataGenerator
{
public class StateRepository:IRepository<State>
{
   		private readonly StateDao _stateDao;
        public StateRepository()
        {
            _stateDao = new StateDao();
        }
        public List<State> GetAllBySortingAndPaged(SortDefinitions sortDefinitions, int startIndex, int pageSize)
        {
            return _stateDao.GetStates(sortDefinitions, startIndex, pageSize);
        }

        public List<State> GetAllBySorting(SortDefinitions sortDefinitions)
        {
            return _stateDao.GetStates(sortDefinitions, -1, 0);
        }

        public List<State> GetAll()
        {
            return _stateDao.GetStates(null, -1, 0);
        }
            public State GetById(object id)
        {
           return _stateDao.GetStateById(id);
        }
	        public long Insert(State state)
        {
           return _stateDao.Insert(state);
        }

        public void Update(State state)
        {
            _stateDao.Update(state);
        }

        public void Delete(State state)
        {
            _stateDao.Delete(state);
        }
}
public class CountryRepository:IRepository<Country>
{
   		private readonly CountryDao _countryDao;
        public CountryRepository()
        {
            _countryDao = new CountryDao();
        }
        public List<Country> GetAllBySortingAndPaged(SortDefinitions sortDefinitions, int startIndex, int pageSize)
        {
            return _countryDao.GetCountries(sortDefinitions, startIndex, pageSize);
        }

        public List<Country> GetAllBySorting(SortDefinitions sortDefinitions)
        {
            return _countryDao.GetCountries(sortDefinitions, -1, 0);
        }

        public List<Country> GetAll()
        {
            return _countryDao.GetCountries(null, -1, 0);
        }
            public Country GetById(object id)
        {
           return _countryDao.GetCountryById(id);
        }
	        public long Insert(Country country)
        {
           return _countryDao.Insert(country);
        }

        public void Update(Country country)
        {
            _countryDao.Update(country);
        }

        public void Delete(Country country)
        {
            _countryDao.Delete(country);
        }
}

}