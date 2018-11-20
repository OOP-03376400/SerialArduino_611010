# SerialArduino_611010
## จากตัวอย่างที่ทดลองในวันนี้ สรุปได้ว่า

* Serial port เป็น control ตัวหนึ่งที่มีมากับชุดพัฒนาโปรแกรม Visual studio เป็น control ที่ไม่เสนอหน้าต่อผู้ใช้ และะทำงานแยกเป็น thread ต่างหาก เป็นอิสระจาก thread ของ User Interface (UI)

* บ่อยครั้งที่เราต้องการนำค่าที่ถูกส่งเข้ามาทาง serial port ไปแสดงในกล่องข้อความ, กล่องรายการลิสต์ เป็นต้น เมื่อเราเขียนโปรแกรมให้เมธอดจาก thread ของ serial port ทำการปรับปรุงค่าของ UI control ก็จะพบว่ามันไม่สามารถทำงานได้ดังต้องการ เพราะตามกฏความปลอดภัยของการใช้ข้อมูลข้ามเธรดแล้ว คนที่จะปรับปรุงค่าใน UI control จะต้องเป็นสมาชิกของ UI control thread เท่านั้น คนอื่นไม่สามารถทำได้

* ดังนั้น เราจะต้องให้เมธอดใน thread ของ serial port ร้องขอ (หรือขอร้อง) ให้เมธอดใน thread ของ UI ทำการปรับปรุงค่าของ control ของตัวมันเอง คล้ายๆ การพูดออกมาว่า "ใครก็ได้ที่อยู่ตรงนั้น ช่วยปรับปรุงค่า control ชื่อนี้ให้หน่อย ให้ค่าเป็นตามนี้นะ" โดยที่ผู้เรียกจะต้องรู้ว่าฟังก์ชันในอีกเธรดหนึ่ง มีหน้าตา (signature) อย่างไรด้วย 

* จากตัวอย่างที่ผ่านมา เราใช้ delegate ซึ่งต้องทำ 4 ขั้นตอนคือ 
1. ประกาศต้นแบบ delegate ที่จะใช้ (คล้ายๆ การประกาศคลาส)
2. ประกาศตัวแปร delegate นั้น
3. สร้าง object จาก delegate ที่ประกาศในข้อ 1 แล้วส่ง reference ให้กับตัวแปรในข้อ 2 พร้อมทั้งระบุเมธอดที่จะทำงานเมื่อเรียกใช้ delegate
4. ใช้งาน delegate ผ่านการ `Invoke()` ของวัตถุในเธรดปลายทาง

**จากตัวอย่างที่ผ่านมา มีความยุ่งยากซับซ้อน (แต่ไม่เกินความสามารถของนักศึกษา)**

แต่เราจะมาลองทำตามตัวอย่างที่ง่ายกว่า และแน่นอนว่า ยังเรียนไม่ถึง


* เมธอดในตัวอย่างข้างล่างนี้ ทำงานเมื่อมีเหตุการณ์รับข้อความเข้ามาทาง serial port โดยผ่านทางเมธอดชื่อ `serialPort1_DataReceived` การส่งข้อความใดๆ เข้ามาทาง serial port นั้น device driver ของระบบ จะนำอักขระที่รับได้ ใส่ไว้ในคิวซึ่งทำหน้าที่เป็น buffer ให้เรา โดยเมธอด `serialPort1_DataReceived` นี้จะยู่ใน thread ของ serial port

* เพื่อการปรับปรุงค่าใน control UI เราจะอ่านอักขระทั้งหมดที่มีอยู่ใน buffer ของ serial port มาเก็บไว้ในตัวแปร `string xx` โดยใช้เมธอดที่ชื่อว่า `ReadExisting()` แล้วทำการส่งไปปรับปรุงค่าของ control ที่ชื่อว่า textBox1 โดยใช้เมธอด `AppendText(xx)`

* ปัญหาที่เราเจอก็คือ serial port กับ `textBox1` อยู่กันคนละ thread ไม่มีทางที่จะส่งข้อความหากันตรงๆ ได้ เราจึงต้องเรียกผ่านเมธอด `this.Invoke` โดยจะไปเรียก "ใครก็ได้" หรือ `()=>` ให้ทำการเพิ่มข้อความใน `textBox1` ของ thread UI

**ไอ้ที่หน้าตาแบบนี้ "()=>" ยังไม่ได้เรียน จะอยู่หน้าท้ายๆ ของชีต week 09** 

* แต่ใครที่อยากทดลองทำไปพลางก่อน ให้พิจารณาตัวอย่างต่อไปนี้
```c#
private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
{
    string xx = serialPort1.ReadExisting();
    this.Invoke(new MethodInvoker(() => { textBox1.AppendText(xx); }));
}
```
* จะเห็นว่าเราสามารถยุบขั้นตอนการใช้งาน delegate ทั้ง 4 ขั้นตอน ให้เหลือเพียงขั้นตอนเดียว 

* เหตุผลที่สามารถยุบเป็น `() => { textBox1.AppendText(xx); }` ได้ก็คือ เนื่องจากเราไม่จำเป็นต้องรู้จักชื่อเมธอดในเธรดอื่น จึงไม่ต้องสนใจว่ามันชื่ออะไร และเมธอดที่ไม่มีชื่อนั้น ไปทำการ invoke เมธอดที่ชื่อ `AppendText( )` ของ `textBox1`

* ตัวอย่างนี้ เราแสดงให้เห็นถึงการใช้งานกับ serial port แต่ในการเชื่อมต่อชนิดอื่นๆ เช่น network หรืออะไรก็แล้วแต่ ก็ใช้งานลักษณะนี้ทั้งหมด เนื่องจากเราต้องแยก thread ระหว่าง คนทำงาน และคนเสนอหน้า ออกจากกัน
