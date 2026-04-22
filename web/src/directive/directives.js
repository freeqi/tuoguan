import { on } from "@/libs/tools";
import { getButtons } from "@/libs/util";
const directives = {
  draggable: {
    inserted: (el, binding, vnode) => {
      let triggerDom = document.querySelector(binding.value.trigger);
      triggerDom.style.cursor = "move";
      let bodyDom = document.querySelector(binding.value.body);
      let pageX = 0;
      let pageY = 0;
      let transformX = 0;
      let transformY = 0;
      let canMove = false;
      const handleMousedown = e => {
        let transform = /\(.*\)/.exec(bodyDom.style.transform);
        if (transform) {
          transform = transform[0].slice(1, transform[0].length - 1);
          let splitxy = transform.split("px, ");
          transformX = parseFloat(splitxy[0]);
          transformY = parseFloat(splitxy[1].split("px")[0]);
        }
        pageX = e.pageX;
        pageY = e.pageY;
        canMove = true;
      };
      const handleMousemove = e => {
        let xOffset = e.pageX - pageX + transformX;
        let yOffset = e.pageY - pageY + transformY;
        if (canMove)
          bodyDom.style.transform = `translate(${xOffset}px, ${yOffset}px)`;
      };
      const handleMouseup = e => {
        canMove = false;
      };
      on(triggerDom, "mousedown", handleMousedown);
      on(document, "mousemove", handleMousemove);
      on(document, "mouseup", handleMouseup);
    },
    update: (el, binding, vnode) => {
      if (!binding.value.recover) return;
      let bodyDom = document.querySelector(binding.value.body);
      bodyDom.style.transform = "";
    }
  },
  permission: {
    // 被绑定元素插入父节点时调用 (仅保证父节点存在，但不一定已被插入文档中)。
    inserted: (el, binding, vnode) => {
      setTimeout(() => {
        let button = getButtons();
        let buttonCode = binding.value;
        let hasPermission = button.some(
          element => element.buttonCode === buttonCode
        );
        // 无权限或者不存在buttonCode值
        if (!hasPermission || !buttonCode) {
          el.parentNode && el.parentNode.removeChild(el);
        }
      }, 60);
    }
  },
  slldrag: {
    inserted: (el, binding, vnode) => {
      let op = el; //获取当前元素
      op.onmousedown = e => {
        //算出鼠标相对元素的位置
        let disX = e.clientX - op.offsetLeft;
        let disY = e.clientY - op.offsetTop;

        let thWidthArr = []; // 记录所有的th的宽度,依次累加
        let finallIndex = 0; //最终的th index
        let directionIndex = 0; // 单击的是第几个th
        let $insertTh = null;

        $insertTh = e.target; //

        const $th = op.parentNode.children; // 获取所有th
        for (let thi = 0; thi < $th.length; thi++) {
          const itemth = $th[thi];
          itemth.index = thi;
          let offsetWidth = 0;
          if (thi === 0) {
            offsetWidth = itemth.offsetWidth + offsetWidth; // 如果是第一个th就不进行累加操作
          } else {
            offsetWidth = itemth.offsetWidth + thWidthArr[thi - 1]; // 累加th的宽度
          }
          thWidthArr.push(offsetWidth);
        }
        directionIndex = e.target.index;
        // 以鼠标按下的这个th处,创建一个和th内容一样的div,
        const $createDiv = document.createElement("div");
        $createDiv.id = "created-div";
        document.getElementById("drag-table").appendChild($createDiv);

        document.onmousemove = e => {
          //用鼠标的位置减去鼠标相对元素的位置，得到元素的位置
          let left = e.clientX - disX;
          let top = e.clientY - disY;
          if (Math.abs(e.clientX - disX - op.offsetLeft) < 10) {
            return;
          }
          //绑定元素位置到positionX和positionY上面
          // this.positionX = top;
          // this.positionY = left;
          //移动当前元素
          const thText = op.innerHTML;
          $createDiv.innerHTML = thText;
          $createDiv.style.position = "absolute";
          $createDiv.style.width = op.offsetWidth + "px";
          $createDiv.style.height = op.offsetHeight + "px";
          $createDiv.style.background = "#666";
          $createDiv.style.left = left + "px";
          $createDiv.style.top = top + "px";

          finallIndex = 0; //鼠标拖动过程中所停留在的th的index
          if (left <= 0) {
            // // 小于0的时候就等于0
            finallIndex = 0;
          } else if (left > thWidthArr[thWidthArr.length - 1]) {
            // 大于最后一个的时候就最后一个+1
            finallIndex = thWidthArr.length;
          } else {
            for (let i = 0; i < thWidthArr.length; i++) {
              if (left >= thWidthArr[i - 1] && left <= thWidthArr[i]) {
                finallIndex = i;
              }
            }
          }
          // 拖动的时候用红线标识拖动到哪个位置
          const $tr = document
            .getElementById("drag-table")
            .getElementsByTagName("table")[0]
            .getElementsByTagName("tr");

          //拖动的时候设置表格的样式
          if (finallIndex === thWidthArr.length) {
            // 如果是最后一个表格就设置右侧的边框
            setStyle(
              $tr,
              finallIndex - 1,
              dom => {
                dom.style.borderColor = "#d8d8d8";
              },
              dom => {
                dom.style.borderRight = "1px solid red";
              }
            );
          } else {
            // 如果不是最后一列表格就设置左侧的边框
            setStyle(
              $tr,
              finallIndex,
              dom => {
                dom.style.borderColor = "#d8d8d8";
              },
              dom => {
                dom.style.borderLeft = "1px solid red";
              }
            );
          }
        };
        document.onmouseup = e => {
          const $tr = document
            .getElementById("drag-table")
            .getElementsByTagName("table")[0]
            .getElementsByTagName("tr");
          document.getElementById("drag-table").removeChild($createDiv);
          // 鼠标抬起恢复表格的样式
          let _finallIndex =
            finallIndex === thWidthArr.length
              ? (_finallIndex = finallIndex - 1)
              : finallIndex; // 如果是拖动到最后一个th
          setStyle(
            $tr,
            _finallIndex,
            dom => {
              dom.style.borderColor = "#d8d8d8";
            },
            dom => {
              dom.style.borderColor = "#d8d8d8";
            }
          );
          // 取消鼠标拖动和鼠标抬起事件
          document.onmousemove = null;
          document.onmouseup = null;

          // 如果没有进行拖动操作(鼠标点下就抬起)
          if (Math.abs(e.clientX - disX - op.offsetLeft) < 10) {
            thWidthArr = [];
            disX = 0;
            return;
          }
          // 遍历tr,将th和td放到最终的位置上
          // const div = document.createElement("div");
          let arr = [];
          for (let tri = 0; tri < $tr.length; tri++) {
            const itemtr = $tr[tri];
            if (itemtr.getElementsByTagName("th").length) {
              const $ths = itemtr.getElementsByTagName("th");
              if (directionIndex > finallIndex) {
                itemtr.insertBefore($insertTh, $ths[finallIndex]);
              } else {
                itemtr.insertBefore($insertTh, $ths[finallIndex + 1]);
              }
              console.log(itemtr.innerHTML);
            }
            if (itemtr.getElementsByTagName("td").length) {
              const $tds = itemtr.getElementsByTagName("td");
              if (directionIndex > finallIndex) {
                itemtr.insertBefore($tds[directionIndex], $tds[finallIndex]);
              } else {
                itemtr.insertBefore(
                  $tds[directionIndex],
                  $tds[finallIndex + 1]
                );
              }
            }
          }
          arr.push($tr);
          localStorage.setItem("changeH", JSON.stringify(arr));

          // 重置thWidthArr和disX
          thWidthArr = [];
          disX = 0;
        };
      };
    }
  }
};

export default directives;
