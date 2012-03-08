using System.Collections.Generic;
using DG.Common;
using DataGenerator;
using NUnit.Framework;

namespace DataGeneratorTest1
{
    [TestFixture]
    class CountryGeneratorsTest
    {
        private CountryRepository _countryRepository;
        [SetUp]
        public void SetUp()
        {
            _countryRepository = new CountryRepository();
        }
        [Test]
        public void GetAllOperations()
        {
            var countries = _countryRepository.GetAll();
            Assert.IsFalse(countries.Count == 0);
        }

        [Test]
        public void GetById()
        {
            var country = _countryRepository.GetById(4);
            Assert.IsFalse(country.Id == 0);
        }
        [Test]
        public void Insert()
        {
            var country = new Country
                              {
                                  HasState = true,
                                  ISO = "BC",
                                  Name = "ABC",
                                  PrintableName = "ABC",
                                  ISO3 = "ABC"
                              };
            long insert = _countryRepository.Insert(country);
            Assert.IsFalse(insert == 0);
        }
        [Test]
        public void Update()
        {
            var country = new Country
                              {
                Id = 245,
                HasState = true,
                ISO = "CC",
                Name = "ABC",
                PrintableName = "ABC",
                ISO3 = "ABC"
            };
            _countryRepository.Update(country);
            Assert.IsFalse(false);
        }
        [Test]
        public void AllbySort()
        {
            var shortItems = new List<SortItem> {new SortItem {ColumnName = "ISO", SortOrder = SortOrder.DESC}};
            var sortDefinitions= new SortDefinitions();
            sortDefinitions.SoftItems.AddRange(shortItems);
            List<Country> allBySorting = _countryRepository.GetAllBySorting(sortDefinitions);
            Assert.IsFalse(allBySorting.Count==0);
        }
        [Test]
        public void SortWithPagination()
        {
            var shortItems = new List<SortItem> { new SortItem { ColumnName = "ISO", SortOrder = SortOrder.DESC } };
            var sortDefinitions = new SortDefinitions();
            sortDefinitions.SoftItems.AddRange(shortItems);
            var allBySorting = _countryRepository.GetAllBySortingAndPaged(sortDefinitions,5,20);
            Assert.IsFalse(allBySorting.Count != 20);
        }
    }
}
