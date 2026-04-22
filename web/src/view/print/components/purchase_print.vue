<template>
  <div class="printbody">
    <div id="purchase_print" class="infor_cont">
      <div class="header">
        <h2 style="text-align: center;">{{`${total_detail.medicalTypeName}采购申请单${total_detail.purchaseNo || '合并明细表'}`}}</h2>
        <h4 style="text-align: left;">注:金额单位（元）</h4>
      </div>
      <table class="table-bordered">
        <tbody>
          <!-- <tr class="headInfo">
            <td colspan="16">
              <p class="items-box">
                <span class="field">申购部门：</span>
                <span>{{total_detail.itemType}}</span>
                <span class="field">申请时间：</span>
                <span>{{total_detail.totalQty || '0'}}件</span>
              </p>
            </td>
          </tr> -->
          <tr v-if="total_detail.medicalTypeName !=='固定资产'">
            <td width="50" class="text_center">序号</td>
            <td width="60" class="text_center">名称</td>
            <td width="60" class="text_center">规格</td>
            <td width="50" class="text_center">月均用量</td>
            <td width="50" class="text_center">当前库存</td>
            <td width="60" class="text_center">单位</td>
            <td width="50" class="text_center">采购数量</td>
            <td width="50" class="text_center">申请数量</td>
            <!-- <td width="60" class="text_center">建议采购单价</td> -->
            <td width="60" class="text_center">采购价</td>
            <!-- <td width="60" class="text_center">社保价</td> -->
            <td width="60" class="text_center">销售价</td>
            <!-- <td width="60" class="text_center">双控价</td> -->
            <td width="60" class="text_center">采购总金额</td>
            <td width="60" class="text_center">销售总金额</td>
            <td width="50" class="text_center" style="white-space: nowrap;">供应商</td>
            <td width="60" class="text_center">厂家</td>
            <td width="80" class="text_center">机构</td>
          </tr>
          <tr v-else>
            <td width="45" class="text_center">序号</td>
            <td width="60" class="text_center">物品名称</td>
            <td width="50" class="text_center">采购数量</td>
            <td width="50" class="text_center">单价预算</td>
            <td width="80" class="text_center">规格型号/技术参数</td>
            <td width="80" class="text_center">申请理由</td>
            <td width="80" class="text_center">备注</td>
          </tr>
          <template v-for="(item,index) in purchase_date">
            <tr v-if="total_detail.medicalTypeName !=='固定资产'" :key="index">
              <td>{{item.no}}</td>
              <td>{{item.medicalItemName}}</td>
              <td>{{item.minDose}}</td>
              <td>{{item.monthAverage}}</td>
              <td>{{item.currentInventory}}</td>
              <td>{{item.procurementUnitValue}}</td>
              <td>{{item.inQty}}</td>
              <td>{{item.approvalQty}}</td>
              <!-- <td>{{item.advicePrice}}</td> -->
              <td>{{item.inPrice}}</td>
              <!-- <td>{{item.socialSecurityPrice}}</td> -->
              <td>{{item.salePrice}}</td>
              <!-- <td>{{item.dualPrice}}</td> -->
              <td>{{item.totalInPrice}}</td>
              <td>{{item.totalSalePrice}}</td>
              <td>{{item.supplierName}}</td>
              <td>{{item.manufacturer}}</td>
              <td>{{item.centerName}}</td>
            </tr>
            <tr v-else :key="index">
              <td>{{item.no}}</td>
              <td>{{item.medicalItemName}}</td>
              <td>{{item.approvalQty}}</td>
              <td>{{item.advicePrice}}</td>
              <td>{{item.specModelsOrMainTechParams}}</td>
              <td>{{item.appReason}}</td>
              <td>{{item.remark}}</td>
            </tr>
          </template>
          <tr class="totalInfo">
            <td :colspan="total_detail.medicalTypeName !=='固定资产' ? '16' : '7'">
              <p style="font-size:15px;text-align:left;padding-left:5px;">合计：</p>
              <p class="items-box">
                <span class="field">物品种类：</span>
                <span>{{total_detail.itemType}}</span>
                <span class="field">合计总数量：</span>
                <span>{{total_detail.totalQty || '0'}}件</span>
                <span class="field">采购总金额：</span>
                <span class="red">￥{{total_detail.actualPrice || '-'}}</span>
                <span class="field">销售总金额：</span>
                <span class="red">￥{{total_detail.salesTotalPrice || '-'}}</span>
              </p>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
<script>
import '@/view/print/print.less'
// import '@/view/print/purchase_print.less'
export default {
  name: 'purchase_print',
  data () {
    return {
      flist: [],
      purchase_date: [],
      total_detail: {}
    }
  },
  created () {
    this.printMerge()
  },
  computed: {},
  methods: {
    printMerge () {
      let purchaseData = sessionStorage.getItem('purchaseData')
      let purchaseDetail = sessionStorage.getItem('purchaseDetail')
      this.purchase_date = JSON.parse(purchaseData)
      this.total_detail = JSON.parse(purchaseDetail)
      window.print()
    }
  }
}
</script>

<style lang="less" scoped="scoped">
#purchase_print {
  .header {
    text-align: center;
    margin-top: 15px;
  }
  .table-bordered {
    text-align: center;
    border-spacing: 0;
    border-collapse: collapse;
    background-color: transparent;
    width: 100%;
    max-width: 100%;
    margin-bottom: 15px;
    td,th {
      padding: 0;
      text-align: center;
      border: 1px solid #aaa !important;
    }
  }
  // .table-bordered > tbody > tr > td {
  //   border: 1px solid #aaa !important;
  // }
  .items-box {
    line-height: 27px;
    font-size: 13px;
    text-align: left;
    padding-left: 5px;
    .field {
      color: #666666;
      & + .red {
        color: #fc4b4b;
      }
    }
    span:not(.field) {
      font-weight: bold;
      margin-right: 50px;
    }
  }
}
</style>
