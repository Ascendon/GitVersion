﻿using System;
using GitVersion;

public class CachedVersion
{
    public SemanticVersion SemanticVersion;
    public long Timestamp;
    public DateTimeOffset MasterReleaseDate;
}