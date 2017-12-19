using DomainModel.Models;
using Repository.IRepo;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Repository.Repo
{
    public class HouseTypeRepository : IHouseTypeRepository
    {
        private readonly IAgrotourismContext db;

        public HouseTypeRepository(IAgrotourismContext db)
        {
            this.db = db;
        }

        public void AddHouseType(HouseType houseType)
        {
            db.HouseTypes.Add(houseType);
        }

        public IList<SelectListItem> getAvaiableTypes()
        {
            IList<HouseType> houseTypes = db.HouseTypes.AsNoTracking().ToList();
            IList<string> avaiableTypes = new List<string>();
            houseTypes.ToList().ForEach(item => avaiableTypes.Add(item.Type));
            IList<SelectListItem> selectList = avaiableTypes.Select(avaiableType => new SelectListItem { Value = avaiableType, Text = avaiableType }).ToList();
            return selectList;
        }

        public HouseType GetHouseTypeById(int id)
        {
            HouseType houseType = db.HouseTypes.Find(id);
            return houseType;
        }

        public HouseType GetHouseTypeByType(string type)
        {
            HouseType houseType = (from item in db.HouseTypes
                                   where item.Type.Equals(type)
                                   select item).FirstOrDefault();
            return houseType;
        }

        public IList<HouseType> GetHouseTypes()
        {
            IQueryable<HouseType> houseTypes = db.HouseTypes.AsNoTracking();
            return houseTypes.ToList();
        }

        public void RemoveHouseType(HouseType houseType)
        {
            db.Entry(houseType).State = EntityState.Deleted;
        }

        public void SaveChanges()
        {
            this.db.SaveChanges();
        }

        public void UpdateHouseType(HouseType houseType, byte[] rowVersion)
        {
            db.Entry(houseType).OriginalValues["RowVersion"] = rowVersion;
        }
    }
}