// var AGYNAME = '重庆山外山康美血液透析中心'
// var AGYNAME_ENG = 'Chongqing SWS KangMei Hemodialysis Center';
(function (view) {
  view.tableExport_new = function (tableId, filename, type) {
    $(function () {
      $('#' + tableId).table2excel({
        exclude: '.noExl',
        name: 'Excel Document Name',
        filename: filename,
        fileext: '.' + type,
        exclude_img: true,
        exclude_links: true,
        exclude_inputs: true
      })
    })
  }
  view.tableExport = function (tableId, filename, type) {
    $(function () {
      $('#' + tableId).table2excel({
        exclude: '.noExl',
        name: 'Excel Document Name',
        filename: filename,
        fileext: '.' + type,
        exclude_img: true,
        exclude_links: true,
        exclude_inputs: true
      })
    })
  }
})(window)
