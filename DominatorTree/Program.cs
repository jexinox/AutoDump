var succ = new List<List<int>>
{
    new(),
    new() { 2, 3 },
    new() { 4, 5, 6 },
    new() { 7 },
    new(),
    new() { 9 },
    new() { 3 },
    new() { 6, 8 },
    new() { 5 },
    new() 
};

// List<List<int>> succ =
// [
//     [],
//     [2, 3],
//     [4, 5],
//     [6],
//     [],
//     [3],
//     [5]
// ];
var n = succ.Count - 1;

var parent = new int[n + 1];
var dom = new int[n + 1];
var vertex = new int[n + 1];
var ancestor = new int[n + 1];

var semi = new int[n + 1];
var label = new int[n + 1];

var pred = new List<List<int>> { new(), new(), new(), new(), new(), new(), new(), new(), new(), new() };
var bucket = new List<List<int>> { new(), new(), new(), new(), new(), new(), new(), new(), new(), new() };
var color = 0;
Dfs(1);
for (var i = n; i > 1; i--)
{
    var w = vertex[i];
    foreach (var v in pred[w])
    {
        var u = Eval(v);
        if (semi[u] < semi[w])
        {
            semi[w] = semi[u];
        }
    }
    bucket[vertex[semi[w]]].Add(w);
    Link(parent[w], w);
    foreach (var v in bucket[parent[w]])
    {
        var u = Eval(v);
        dom[v] = semi[u] < semi[v] ? u : parent[w];
    }
}

for (var i = 2; i <= n; i++)
{
    var w = vertex[i];
    if (dom[w] != vertex[semi[w]])
    {
        dom[w] = dom[dom[w]];
    }
}

dom[0] = 0;

foreach (var dominator in dom)
{
    Console.WriteLine(dominator);
}

return;

void Dfs(int v)
{
    semi[v] = ++color;
    label[v] = v;
    vertex[color] = v;
    ancestor[v] = 0;
    foreach (var w in succ[v])
    {
        if (semi[w] == 0)
        {
            parent[w] = v;
            Dfs(w);
        }
        pred[w].Add(v);
    }
}

void Compress(int v)
{
    if (ancestor[ancestor[v]] != 0)
    {
        Compress(ancestor[v]);
        if (semi[label[ancestor[v]]] < semi[label[v]])
        {
            label[v] = label[ancestor[v]];
        }

        ancestor[v] = ancestor[ancestor[v]];
    }
}

int Eval(int v)
{
    if (ancestor[v] == 0)
    {
        return v;
    }

    Compress(v);
    return label[v];
}

void Link(int v, int w)
{
    ancestor[w] = v;
}