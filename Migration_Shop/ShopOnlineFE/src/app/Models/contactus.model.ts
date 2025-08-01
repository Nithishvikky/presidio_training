export interface ContactUsRequest {
  customerName: string;
  customerEmail: string;
  customerPhone: string;
  customerContent: string;
  captchaToken: string;
}
