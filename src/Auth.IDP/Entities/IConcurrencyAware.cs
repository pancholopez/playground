﻿namespace Auth.IDP.Entities;

public interface IConcurrencyAware
{
    string ConcurrencyStamp { get; set; }
}