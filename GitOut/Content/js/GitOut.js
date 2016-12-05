var getRepos = p => getData(p, "/repos", x => x, r => r.name, (e, r) => getForks(e, r));

var getForks = (p, r) => {
  p.append($(`<h2>${r.name}</h2>`));
  getData(p, `/forks/${encode(r.name)}`, x => x.forks, f => f.owner, (e, f) => getBranches(e, r, f));
};

var getBranches = (p, r, f) => {
  var owner = new RegExp("[\?&]o=([^&#]*)").exec(window.location.href);
  if (owner && f.owner !== owner[1]) {
    p.children(".loader").remove();
    return;
  }

  const uri = `/branches/${encode(r.name)}/${encode(f.owner)}`;
  getData(p, uri, x => x.branches, b => `${f.owner}/${b.name}`, (e, b) => getDiff(e, r, f, b));
};

var getDiff = (p, r, f, b) => {
  const uri = `/diff/${encode(r.name)}/${encode(f.owner)}/${encode(b.name)}`;
  getData(p, uri, null, null, null, d => printDiff(p, r, f, b, d));
};

var printDiff = (p, r, f, b, d) => {
  if (d.status === "identical") return;
  if (d.status === "behind") return;

  p.append($(`<div class="branch">
    <span class ="status" style="background-color: #ff9999">
      <a href="/?o=${f.owner}">${f.owner}</a>'s ${b.name}
    </span>
    <span class="link"><a href="${d.link}">Check PR</a></span>
    <span class="info">by ${d.by} commits</span>
    <br class ="clear" />
    </div>`));
};

var encode = x => encodeURIComponent(x.replace("#", "x___x").replace("\\", "x____x").replace("/", "x_____x"));

var getData = (p, uri, s, id, l, cb) => {
  $.getJSON(uri,
    x => {
      p.children(".loader").remove();

      if (x.message && 0 !== x.message.length) return false;
      if (cb) return cb(x);

      var items = s(x);

      return items.forEach(i => {
        var e = $(`<div><div class="loader">${id(i)} <img src="Content/img/loader.gif" /></div></div>`);
        p.append(e.ready(() => l(e, i)));
      });
    });
};

$("document").ready(() => getRepos($("#data")));