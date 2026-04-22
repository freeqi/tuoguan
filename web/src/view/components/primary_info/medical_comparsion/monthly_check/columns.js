export default {
  data() {
    return {
      columns: [
        {
          title: "目录",
          key: "medCatalog",
          fixed: "left",
          width: 90,
          tooltip: true
        },
        {
          title: "名称",
          key: "medicalItemName",
          tooltip: true,
          align: "center",
          width: 200
        },
        {
          title: "编码",
          key: "medicalItemCode",
          tooltip: true,
          align: "center",
          width: 120
        },
        {
          title: "包装",
          key: "packaging",
          tooltip: true,
          minWidth: 100
        },
        {
          title: "单位",
          key: "medUnit",
          tooltip: true,
          width: 60
        },
        {
          title: "批准文号",
          key: "approvalNum",
          tooltip: true,
          minWidth: 100
        },
        {
          title: "厂家",
          key: "manufacturer",
          tooltip: true,
          minWidth: 140
        },
        {
          title: "国家码",
          key: "nationItemCode",
          // tooltip: true,
          minWidth: 160
        },
        {
          title: "采购价",
          key: "purchasingPrice",
          // tooltip: true,
          width: 90
        },
        {
          title: "销售价/限价",
          key: "retailPrice",
          // tooltip: true,
          width: 90
        },
        {
          title: "目录等级",
          key: "ylfydj",
          align: "center",
          width: 100,
          tooltip: true,
          render: (h, params) => {
            let text = this.medical_insurance_type[params.row.ylfydj - 1];
            return <span>{text || "丙"}级</span>;
          }
        },
        {
          title: "审核状态",
          width: 80,
          key: "auditState",
          align: "center",
          render: (h, params) => {
            if (params.row.auditState === -1) {
              return (
                <i-button size="small" disabled>
                  医保目录
                </i-button>
              );
            } else if (params.row.auditState === 0) {
              return (
                <i-button
                  size="small"
                  style="color: #fff;background-color: #3CADD9;border-color: #3CADD9;"
                  onClick={() => {
                    this.auditSubmit(params.row);
                  }}
                >
                  未审核
                </i-button>
              );
            } else if (params.row.auditState === 1) {
              return (
                <i-button
                  size="small"
                  style="color: #fff;background-color: #19be6b;border-color: #19be6b;"
                  onClick={() => {
                    this.auditSubmit(params.row);
                  }}
                >
                  已通过
                </i-button>
              );
            } else if (params.row.auditState === 2) {
              return (
                <i-button
                  size="small"
                  style="color: #fff;background-color: #ed4014;border-color: #ed4014;"
                  onClick={() => {
                    this.auditSubmit(params.row);
                  }}
                  type="success"
                >
                  未通过
                </i-button>
              );
            }
          }
        }
      ]
    };
  }
};
