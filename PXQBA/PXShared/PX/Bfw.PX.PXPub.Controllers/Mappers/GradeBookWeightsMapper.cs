using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class GradeBookWeightsMapper
    {

        /// <summary>
        /// Converts to a Grade Book Weight from a Biz Grade Book Weight.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <returns></returns>
        public static GradeBookWeights ToGradeBookWeights(this BizDC.GradeBookWeights biz)
        {
            var model = new GradeBookWeights();

            model.CategoryWeightTotal = biz.CategoryWeightTotal;
            model.Total = biz.Total;
            model.TotalWithExtraCredit = biz.TotalWithExtraCredit;
            model.WeightedCategories = biz.WeightedCategories;

            if (!biz.GradeWeightCategories.IsNullOrEmpty())
            {
                model.GradeWeightCategories = biz.GradeWeightCategories.Map(c => c.ToGradeBookWeightCategory()).ToList();
            }

            return model;
        }

        /// <summary>
        /// Converts to a Grade Book Weight from a course object. (performance optimization)
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <returns></returns>
        public static GradeBookWeights ToGradeBookWeights(this BizDC.Course course)
        {
            var model = new GradeBookWeights();
            if (course.GradeBookCategoryList != null)
            {
                model.CategoryWeightTotal = course.GradeBookCategoryList.Sum(c =>
                    {
                        try
                        {
                            return float.Parse(c.Weight);
                        }
                        catch
                        {
                            return 0;
                        }
                    });

                model.Total = course.GradeBookCategoryList.Sum(c =>
                    {
                        try
                        {
                            return float.Parse(c.ItemWeightTotal);
                        }
                        catch
                        {
                            return 0;
                        }
                    });

                model.TotalWithExtraCredit = course.GradeBookCategoryList.Sum(c =>
                {
                    try
                    {
                        return float.Parse(c.PercentWithExtraCredit);
                    }
                    catch
                    {
                        return 0;
                    }
                });
            }
            model.WeightedCategories = course.UseWeightedCategories;

            if (!course.GradeBookCategoryList.IsNullOrEmpty())
            {
                model.GradeWeightCategories = course.GradeBookCategoryList.Map(c => c.ToGradeBookWeightCategory()).ToList();
            }

            return model;
        }

    }
}
