﻿// -----------------------------------------------------------------------
//  <copyright file="EntityInfoController.cs" company="OSharp开源团队">
//      Copyright (c) 2014-2018 OSharp. All rights reserved.
//  </copyright>
//  <site>http://www.osharp.org</site>
//  <last-editor>郭明锋</last-editor>
//  <last-date>2018-03-10 17:00</last-date>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OSharp.AspNetCore.Mvc.Filters;
using OSharp.AspNetCore.UI;
using OSharp.Core.EntityInfos;
using OSharp.Core.Modules;
using OSharp.Data;
using OSharp.Demo.Security;
using OSharp.Demo.Security.Dtos;
using OSharp.Entity;
using OSharp.Filter;
using OSharp.Security;


namespace OSharp.Demo.WebApi.Areas.Admin.Controllers
{
    [ModuleInfo(Order = 5, Position = "Security")]
    [Description("管理-实体信息")]
    public class EntityInfoController : AdminApiController
    {
        private readonly SecurityManager _securityManager;

        public EntityInfoController(SecurityManager securityManager)
        {
            _securityManager = securityManager;
        }

        /// <summary>
        /// 读取实体信息
        /// </summary>
        /// <returns>实体信息集合</returns>
        [HttpPost]
        [ModuleInfo]
        [AllowAnonymous]
        [Description("读取")]
        public PageData<EntityInfoOutputDto> Read()
        {
            PageRequest request = new PageRequest(Request);
            if (request.PageCondition.SortConditions.Length == 0)
            {
                request.PageCondition.SortConditions = new[] { new SortCondition("TypeName") };
            }
            Expression<Func<EntityInfo, bool>> predicate = FilterHelper.GetExpression<EntityInfo>(request.FilterGroup);
            var page = _securityManager.EntityInfos.ToPage<EntityInfo, EntityInfoOutputDto>(predicate, request.PageCondition);
            return page.ToPageData();
        }

        [HttpPost]
        [ModuleInfo]
        [DependOnFunction("Read")]
        [ServiceFilter(typeof(UnitOfWorkAttribute))]
        [Description("更新")]
        public async Task<IActionResult> Update(EntityInfoInputDto[] dtos)
        {
            Check.NotNull(dtos, nameof(dtos));
            OperationResult result = await _securityManager.UpdateEntityInfos(dtos);
            return Json(result.ToAjaxResult());
        }
    }
}