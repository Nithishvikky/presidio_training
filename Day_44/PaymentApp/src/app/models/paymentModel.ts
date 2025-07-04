export class PaymentModel{
  constructor(
    public paymentId:string="",
    public Name:string="",
    public Number:string="",
    public Email:string="",
    public Amount:number=0,
    public PaymentDate:Date = new Date()
  ){}
}
