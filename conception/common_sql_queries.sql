-- recup cat / subcat
select * from f_category_type as ct;
select ca.ca_fk_category_type_id,ca.ca_name from f_category as ca;

select * from f_intervention as i where i.i_fk_client_id=2 order by i.i_date desc limit 1;

select c.c_id,concat(concat(c.c_name,' '),c.c_firstname),c.c_location_lat,c.c_location_long,c.c_email from f_client as c
inner join f_intervention as i on i.i_fk_client_id=c.c_id
inner join f_invoice as inv on inv.in_id=i.i_fk_invoice_id
where c.c_is_pro='0' and date_part('year',inv.in_receipt_date)=2022
group by c.c_id
order by c.c_travel_time;

select concat(concat(c.c_name,' '),c.c_firstname) from f_client as c
inner join f_intervention as i on i.i_fk_client_id=c.c_id
inner join f_invoice as inv on inv.in_id=i.i_fk_invoice_id
where c.c_is_pro='0' and date_part('year',inv.in_receipt_date)=2022 and c.c_email=''
group by c.c_id
order by c.c_travel_time;