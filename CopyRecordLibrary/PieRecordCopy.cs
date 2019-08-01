using System;
using Terrasoft.Core;
using Terrasoft.Core.DB;
using Terrasoft.Core.Entities;

namespace CopyRecordLibrary
{
    public class PieRecordCopy
    {
        private UserConnection _userConnection;

        public PieRecordCopy(UserConnection uc)
        {
            _userConnection = uc;
        }
        public string CopyRec(Guid pieRecId)
        {
            var id = Guid.NewGuid();
            var select = new Select(_userConnection)
                    .Column(Column.Const(id))
                    .Column("UsrName")
                    .Column("UsrPieS1Id")
                    .Column("UsrDateS")
                    .Column("UsrWt")
                    .Column("UsrCountPie")
                    .Column("UsrContOrderId")
                    .Column("UsrWish")
                    .Column("UsrClientCart")
                    .Column("UsrNaturJe")
                .From("UsrPie")
                .Where("Id").IsEqual(Column.Parameter(pieRecId)) as Select;
            var insel = new InsertSelect(_userConnection)
                .Into("UsrPie")
                    .Set("Id", "UsrName", "UsrPieS1Id", "UsrDateS", "UsrWt", "UsrCountPie", "UsrContOrderId", "UsrWish", "UsrClientCart", "UsrNaturJe")
                .FromSelect(select);
            insel.Execute();

            var selectDetail = new Select(_userConnection)
                   .Column("UsrVeusFilling")
                   .Column("UsrNLayers")
                   .Column("UsrColorLayer")
                   .Column(Column.Const(id))
               .From("UsrFillingPie")
               .Where("UsrOrderVeusId").IsEqual(Column.Parameter(pieRecId)) as Select;
            var inselDetail = new InsertSelect(_userConnection)
                .Into("UsrFillingPie")
                    .Set("UsrVeusFilling", "UsrNLayers", "UsrColorLayer", "UsrOrderVeusId")
                .FromSelect(selectDetail);
            inselDetail.Execute();

            return string.Empty; 
        }

        public string CopyRecDouble(Guid pieRecId)
        {
            var id = Guid.NewGuid();
            var entity = _userConnection.EntitySchemaManager.GetInstanceByName("UsrPie");
            var pie = entity.CreateEntity(_userConnection);
            var pie1 = entity.CreateEntity(_userConnection);
            var entityDelail = _userConnection.EntitySchemaManager.GetInstanceByName("UsrFillingPie");

            
            bool result = pie.FetchFromDB(pieRecId);
            if (result)
            {
                pie1.SetColumnValue("Id", id);
                pie1.SetColumnValue("UsrName",pie.GetColumnValue("UsrName"));
                pie1.SetColumnValue("UsrPieS1Id", pie.GetColumnValue("UsrPieS1Id"));
                pie1.SetColumnValue("UsrDateS", pie.GetColumnValue("UsrDateS"));
                pie1.SetColumnValue("UsrWt", pie.GetColumnValue("UsrWt"));
                pie1.SetColumnValue("UsrCountPie", pie.GetColumnValue("UsrCountPie"));
                pie1.SetColumnValue("UsrContOrderId", pie.GetColumnValue("UsrContOrderId"));
                pie1.SetColumnValue("UsrWish", pie.GetColumnValue("UsrWish"));
                pie1.SetColumnValue("UsrClientCart", pie.GetColumnValue("UsrClientCart"));
                pie1.SetColumnValue("UsrNaturJe", pie.GetColumnValue("UsrNaturJe"));
                pie1.Save();
            }

            EntitySchemaManager esqManager = _userConnection.EntitySchemaManager;
            var esqResult = new EntitySchemaQuery(esqManager, "UsrFillingPie");
                esqResult.AddColumn("UsrVeusFilling");
                esqResult.AddColumn("UsrNLayers");
                esqResult.AddColumn("UsrColorLayer");
            var esqFunction = esqResult.CreateFilterWithParameters(FilterComparisonType.Equal,
                    "UsrOrderVeus.Id",
                    pieRecId);
            esqResult.Filters.Add(esqFunction);
            var entities = esqResult.GetEntityCollection(_userConnection);
            foreach (var ent in entities)
            {
                var pieDetail = entityDelail.CreateEntity(_userConnection);
                pieDetail.SetColumnValue("Id", Guid.NewGuid());
                pieDetail.SetColumnValue("UsrVeusFilling", ent.GetColumnValue("UsrVeusFilling"));
                pieDetail.SetColumnValue("UsrNLayers", ent.GetColumnValue("UsrNLayers"));
                pieDetail.SetColumnValue("UsrColorLayer", ent.GetColumnValue("UsrColorLayer"));
                pieDetail.SetColumnValue("UsrOrderVeusId", id);
                pieDetail.Save();
            }
            return string.Empty;
        }
    }
}
