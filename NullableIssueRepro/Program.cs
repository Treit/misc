var ids = GetIds();

if ((ids?.Count ?? 0) == 0)
{
    return;
}

if (ids is null || ids.Count == 0)
{
    return;
}

DoSomething(ids);

static void DoSomething(List<string> ids)
{
}

static List<string>? GetIds()
{
    return null;
}