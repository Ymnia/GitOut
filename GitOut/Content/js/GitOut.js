var loader = "<img class='loader' src='Content/img/loader.gif' />";
var repos, forks, branches;

var getDiff = function(parent, r, f, b) {
  $.getJSON("/diff/" + encodeURIComponent(r.name) + "/" + encodeURIComponent(f.owner) + "/" + encodeURIComponent(b.name),
    function(x) {
      parent.children(".loader").remove();
      if (!x.Error) parent.append($("<span>" + x.status + "</span>"));
    });
};

var getBranches = function(parent, r, f) {
  $.getJSON("/branches/" + encodeURIComponent(r.name) + "/" + encodeURIComponent(f.owner),
    function(x) {
      parent.children(".loader").remove();
      (branches = x.branches).forEach(function(b) {
        var child = $("<div><h4>" + b.name + "</h4>" + loader + "</div>");
        parent.append(child.ready(function() { getDiff(child, r, f, b) }));
      });
    });
};

var getForks = function(parent, r) {
  $.getJSON("/forks/" + encodeURIComponent(r.name),
    function(x) {
      parent.children(".loader").remove();
      (forks = x.forks).forEach(function(f) {
        var child = $("<div><h3>" + f.owner + "</h3>" + loader + "</div>");
        parent.append(child.ready(function() { getBranches(child, r, f) }));
      });
    });
};

var getRepos = function(parent) {
  $.getJSON("/repos",
    function(x) {
      parent.children(".loader").remove();
      (repos = x).forEach(function(r) {
        var child = $("<div><h2>" + r.name + "</h2>" + loader + "</div>");
        parent.append(child.ready(function() { getForks(child, r) }));
      });
    });
};

$("document").ready(function () { getRepos($("#data")) });