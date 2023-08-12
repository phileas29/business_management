﻿using FM.Domain.Models.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FM.Domain.Abstractions.Service
{
    public interface ICategoryService
    {
        Task<SelectList> GetSelectListAsync(string categoryType);
    }
}
